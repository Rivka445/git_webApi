using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class DressRepository : IDressRepository
    {
        private readonly EventDressRentalContext _eventDressRentalContext;
        public DressRepository(EventDressRentalContext eventDressRentalContext)
        {
            _eventDressRentalContext = eventDressRentalContext;
        }
        public async Task<bool> IsExistsDressById(int id)
        {
            return await _eventDressRentalContext.Dresses.AnyAsync(d => d.Id == id);
        }
        public async Task<bool> IsDressAvailable(int id, DateOnly date)
        {
            var isDressAvailable = await _eventDressRentalContext.Dresses
                .Where(d => d.Id == id && d.IsActive == true)
                .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.Order)
                .Where(d => !d.OrderItems.Any(oi =>
                    oi.Order.EventDate >= date.AddDays(-7) &&
                    oi.Order.EventDate <= date.AddDays(7)))
                .AnyAsync();
            return isDressAvailable;
        }
        public async Task<Dress?> GetDressById(int id)
        {
            return await _eventDressRentalContext.Dresses
                .Include(d => d.Model)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive == true);
        }
        public async Task<List<string>> GetSizesByModelId(int modelId)
        {
            return await _eventDressRentalContext.Dresses
                .Where(d => d.IsActive == true && d.ModelId == modelId)
                .Select(d => d.Size)
                .Distinct()
                .ToListAsync();
        }
        public async Task<int> GetPriceById(int id)
        {
            return await _eventDressRentalContext.Dresses
            .Where(d => d.Id == id)
            .Select(d => d.Price)
            .FirstOrDefaultAsync();
        }
        public async Task<int> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date)
        {
            var dressesCount = await _eventDressRentalContext.Dresses
                .Where(d =>  d.IsActive == true && d.ModelId == modelId && d.Size == size )
                .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.Order)
                .Where(d => !d.OrderItems.Any(oi =>
                    oi.Order.EventDate >= date.AddDays(-7) &&
                    oi.Order.EventDate <= date.AddDays(7)))
                .CountAsync();
           return dressesCount;
        }
        public async Task<Dress> AddDress(Dress dress)
        {
            await _eventDressRentalContext.Dresses.AddAsync(dress);
            await _eventDressRentalContext.SaveChangesAsync();
            return dress;
        } 
        public async Task UpdateDress(Dress dress)
        {
             _eventDressRentalContext.Dresses.Update(dress);
            await _eventDressRentalContext.SaveChangesAsync();
        }
        public async Task DeleteDress(int id)
        {
            await _eventDressRentalContext.Dresses
                .Where(d => d.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsActive, false)
                );
        }
    }
}
