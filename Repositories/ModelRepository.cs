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
            return await _eventDressRentalContext.Models.AnyAsync(m => m.Id == id);
        }
        public async Task<Model?> GetModelById(int id)
        {
            return await _eventDressRentalContext.Models
                .Include(m => m.Categories)
                .Include(m => m.Dresses)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsActive == true);
        }
        public async Task<(List<Model> Items, int TotalCount)> GetModels(string? description, int? minPrice, int? maxPrice,
            int[] categoriesId, string? color, int position=1, int skip=8)
        {
            var query = _eventDressRentalContext.Models.Where(product =>
            product.IsActive == true
            &&(description == null ? (true) : (product.Description.Contains(description)))
            && ((minPrice == null) ? (true) : (product.BasePrice >= minPrice))
            && ((maxPrice == null) ? (true) : (product.BasePrice <= maxPrice))
            && ((color == null) ? (true) : (product.Color == color))
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
        public async Task<Model> AddModel(Model model)
        {
            await _eventDressRentalContext.Models.AddAsync(model);
            foreach (var category in model.Categories)
            {
                _eventDressRentalContext.Entry(category).State = EntityState.Unchanged;
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
            await _eventDressRentalContext.Models
            .Where(d => d.Id == id)
            .ExecuteUpdateAsync(s => s
            .SetProperty(d => d.IsActive, false)
            );
        }
    }
}