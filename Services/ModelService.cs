using AutoMapper;
using Entities;
using DTOs;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _modelRepository;
        private readonly IDressService _dressService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public ModelService(IModelRepository modelRepository, IMapper mapper, IDressService dressService, ICategoryService categoryService)
        {
            _mapper = mapper;
            _dressService = dressService;
            _modelRepository = modelRepository;
            _categoryService = categoryService;
        }
        public async Task<bool> IsExistsModelById(int id)
        {
            return await _modelRepository.IsExistsModelById(id);
        }
        public async Task<bool> checkCategories(List<NewCategoryDTO> categories)
        {
            foreach (var category in categories)
            {
                if(!await _categoryService.IsExistsCategoryById(category.Id))
                    return false;
            }
            return true;
        }
        public bool checkPrice(int price)
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
            int[] categoriesId, string? color, int position = 1, int skip = 8)
        {
            (List<Model> Items, int TotalCount) products = await _modelRepository
                        .GetModels(description, minPrice, maxPrice, categoriesId, color, position, skip);
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
            return finalProducts;
        }
        public async Task<ModelDTO> AddModel(NewModelDTO newModel)
        {
            Model addedModel = _mapper.Map<NewModelDTO, Model>(newModel);
            Model model = await _modelRepository.AddModel(addedModel);
            ModelDTO modelDTO = _mapper.Map<Model, ModelDTO>(model);
            return modelDTO;
        }
        public async Task UpdateModel(int id, ModelDTO updateModel)
        {
            Model update = _mapper.Map<ModelDTO, Model>(updateModel);
            await _modelRepository.UpdateModel(update);
        }
        public async Task DeleteModel(int id, ModelDTO deleteModel)
        {
            Model model = _mapper.Map<ModelDTO, Model>(deleteModel);
            model.IsActive = false;
            foreach (var dress in model.Dresses)
            {
                DressDTO dressDTO = _mapper.Map<Dress, DressDTO>(dress);
                await _dressService.DeleteDress(dress.Id, dressDTO);
            }
            await _modelRepository.DeleteModel(model);
        }
    }
}
