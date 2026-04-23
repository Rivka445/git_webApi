using AutoMapper;
using Entities;
using DTOs;
using Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;
using StackExchange.Redis;

namespace Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;
        private readonly IDressService _dressService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public ModelService(IModelRepository modelRepository, IMapper mapper, IDressService dressService,
             ICategoryService categoryService, IDistributedCache cache, IConfiguration configuration)
        {
            _mapper = mapper;
            _dressService = dressService;
            _modelRepository = modelRepository;
            _categoryService = categoryService;
            _cache = cache;
            _configuration = configuration;
        }
        public async Task<bool> IsExistsModelById(int id)
        {
            return await _modelRepository.IsExistsModelById(id);
        }
        public bool CheckDate(DateOnly date)
        {
            return date > DateOnly.FromDateTime(DateTime.Now);
        }
        public async Task<bool> CheckCategories(List<int> categories)
        {
            foreach (var category in categories)
            {
                if (!await _categoryService.IsExistsCategoryById(category))
                    return false;
            }
            return true;
        }
        public bool CheckPrice(int price)
        {
            return price > 0;
        }
        public bool ValidateQueryParameters(int position, int skip, int? minPrice, int? maxPrice)
        {
            if(minPrice.HasValue && maxPrice.HasValue)
                return position >= 0 && skip >= 0 && minPrice < maxPrice;
            return position >= 0 && skip >= 0;
        }
        public async Task<ModelDTO> GetModelById(int id)
        {
            Model? model = await _modelRepository.GetModelById(id);
            if (model == null)
                return null;
            ModelDTO modelDTO = _mapper.Map<Model, ModelDTO>(model);
            return modelDTO;
        }
        public async Task<FinalModels> GetModelds(string? description, int? minPrice, int? maxPrice,
            int[] categoriesId, string[] colors, int position = 1, int skip = 8)
        {
            string categoriesString = categoriesId != null ? string.Join("-", categoriesId) : "";
            string cacheKey = $"Models_pos{position}_skip{skip}_min{minPrice}_max{maxPrice}_cat{categoriesString}_desc{description}";
            string? cachedJson = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                var cachedResult = JsonSerializer.Deserialize<FinalModels>(cachedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (cachedResult != null && cachedResult.Items != null && cachedResult.Items.Any())
                    return cachedResult;
            }
            (List<Model> Items, int TotalCount) products = await _modelRepository
                        .GetModels(description, minPrice, maxPrice, categoriesId, colors, position, skip);
            List<ModelDTO> productsDTO = _mapper.Map<List<Model>, List<ModelDTO>>(products.Items);
            bool hasNext = (products.TotalCount - (position * skip)) > 0;
            bool hasPrev = position > 1;
            FinalModels finalProducts = new()
            {
                Items = productsDTO,
                TotalCount = products.TotalCount,
                HasNext = hasNext,
                HasPrev = hasPrev
            };
            var ttlFromConfig = _configuration.GetValue<int>("RedisCacheOptions:TTL_In_Seconds", 3600);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlFromConfig)
            };
            var json = JsonSerializer.Serialize(finalProducts, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await _cache.SetStringAsync(cacheKey, json, options);

            return finalProducts;
        }
        public async Task<List<string>> GetSizesByModelId(int modelId)
        {
            return await _modelRepository.GetSizesByModelId(modelId);
        }

        public async Task<int> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date)
        {
            return await _modelRepository.GetCountByModelIdAndSizeForDate(modelId, size, date);
        }
        public async Task<ModelResponseDTO> AddModel(NewModelDTO newModel)
        {
            Model addedModel = _mapper.Map<NewModelDTO, Model>(newModel);
            addedModel.IsActive = true;
            Model model = await _modelRepository.AddModel(addedModel);
            ModelResponseDTO modelDTO = _mapper.Map<Model, ModelResponseDTO>(model);
            var connection = await ConnectionMultiplexer.ConnectAsync(_configuration.GetConnectionString("Redis"));
            var server = connection.GetServer(connection.GetEndPoints().First());
            var database = connection.GetDatabase();
            var keys = server.Keys(pattern: "*Models_*").ToArray();
            foreach (var key in keys)
            {
                await database.KeyDeleteAsync(key);
            }
            return modelDTO;
        }
        public async Task UpdateModel(int id, NewModelDTO updateModel)
        {
            Model update = _mapper.Map<NewModelDTO, Model>(updateModel);
            update.Id = id;
            update.IsActive = true;
            await _modelRepository.UpdateModel(update);
            var connection = await ConnectionMultiplexer.ConnectAsync(_configuration.GetConnectionString("Redis"));
            var server = connection.GetServer(connection.GetEndPoints().First());
            var database = connection.GetDatabase();
            var keys = server.Keys(pattern: "*Models_*").ToArray();
            foreach (var key in keys)
            {
                await database.KeyDeleteAsync(key);
            }
        }
        public async Task DeleteModel(int id)
        {
            Model? model = await _modelRepository.GetModelById(id);
            foreach (var dress in model.Dresses)
            {
                DressDTO dressDTO = _mapper.Map<Dress, DressDTO>(dress);
                await _dressService.DeleteDress(dress.Id);
            }
            await _modelRepository.DeleteModel(id);
            var connection = await ConnectionMultiplexer.ConnectAsync(_configuration.GetConnectionString("Redis"));
            var server = connection.GetServer(connection.GetEndPoints().First());
            var database = connection.GetDatabase();
            var keys = server.Keys(pattern: "*Models_*").ToArray();
            foreach (var key in keys)
            {
                await database.KeyDeleteAsync(key);
            }
        }
    }
}
