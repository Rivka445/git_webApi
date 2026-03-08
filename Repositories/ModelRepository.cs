using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ModelRepository : IModelRepository
    {
        private readonly EventDressRentalContext _eventDressRentalContext;
        public ModelRepository(EventDressRentalContext eventDressRentalContext)
        {
            _eventDressRentalContext = eventDressRentalContext;
        }
        public async Task<bool> IsExistsModelById(int id)
        {
            return await _eventDressRentalContext.Models.AnyAsync(m => m.Id == id && m.IsActive == true);
        }
        public async Task<Model?> GetModelById(int id)
        {
            return await _eventDressRentalContext.Models
                .Include(m => m.Categories)
                .Include(m => m.Dresses)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive == true);
        }
        public async Task<(List<Model> Items, int TotalCount)> GetModels(string? description, int? minPrice, int? maxPrice,
            int[] categoriesId, string[] colors, int position=1, int skip=8)
        {
            var query = _eventDressRentalContext.Models.Where(product =>
            product.IsActive == true
            &&(description == null ? (true) : (product.Name.Contains(description)))
            && ((minPrice == null) ? (true) : (product.BasePrice >= minPrice))
            && ((maxPrice == null) ? (true) : (product.BasePrice <= maxPrice))
            && (colors.Count() == 0) ? (true) : (colors.Contains(product.Color))
            && ((categoriesId.Count() == 0) ? (true) : product.Categories.Any(c => categoriesId.Contains(c.Id))))
                .OrderBy(product => product.BasePrice);
                Console.WriteLine(query.ToQueryString());
                List<Model> products = await query.Skip((position - 1) * skip)
                .Take(skip)
                .Include(product => product.Categories)
                .ToListAsync();
            var total = await query.CountAsync();
            return (products, total);
        }
        public async Task<List<string>> GetSizesByModelId(int modelId)
        {
            return await _eventDressRentalContext.Dresses
                .Where(d => d.IsActive == true && d.ModelId == modelId)
                .Select(d => d.Size)
                .Distinct()
                .ToListAsync();
        }
        public async Task<int> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date)
        {
            var dressesCount = await _eventDressRentalContext.Dresses
                .Where(d => d.IsActive == true && d.ModelId == modelId && d.Size == size)
                .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.Order)
                .Where(d => !d.OrderItems.Any(oi =>
                    oi.Order.EventDate >= date.AddDays(-7) &&
                    oi.Order.EventDate <= date.AddDays(7)))
                .CountAsync();
            return dressesCount;
        }
        public async Task<Model> AddModel(Model model)
        {
            await _eventDressRentalContext.Models.AddAsync(model);
            foreach (var category in model.Categories)
            {
                _eventDressRentalContext.Categories.Attach(category);
            }
            await _eventDressRentalContext.SaveChangesAsync();
            return model;
        }
        public async Task UpdateModel(Model model)
        {
            var existingModel = await _eventDressRentalContext.Models
                .Include(m => m.Categories)
                .FirstOrDefaultAsync(m => m.Id == model.Id);
            existingModel.Name = model.Name;
            existingModel.Description = model.Description;
            existingModel.ImgUrl = model.ImgUrl;
            existingModel.BasePrice = model.BasePrice;
            existingModel.Color = model.Color;
            existingModel.IsActive = model.IsActive; existingModel.Categories.Clear();
            foreach (var category in model.Categories)
            {
                var existingCategory = await _eventDressRentalContext.Categories
                    .FindAsync(category.Id);
                existingModel.Categories.Add(existingCategory);
            }
            await _eventDressRentalContext.SaveChangesAsync();
        }
        public async Task DeleteModel(int id)
        {
            Model? model = await _eventDressRentalContext.Models
               .FirstOrDefaultAsync(m => m.Id == id);
            model.IsActive = false;
            await _eventDressRentalContext.SaveChangesAsync();
        }
    }
}