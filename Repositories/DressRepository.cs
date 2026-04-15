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
            return await _eventDressRentalContext.Dresses.AnyAsync(d => d.Id == id && d.IsActive == true);
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
        public async Task<List<Dress>> GetDresses()
        {
            return await _eventDressRentalContext.Dresses
                .Include(d => d.Model)
                .Where(d => d.IsActive == true)
                .ToListAsync();
        }
        public async Task<Dress?> GetDressById(int id)
        {
            return await _eventDressRentalContext.Dresses
                .Include(d => d.Model)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive == true);
        }
        public async Task<Dress?> GetDressByModelIdAndSize(int modelId, string size)
        {
            return await _eventDressRentalContext.Dresses
                .Include(d => d.Model)
                .FirstOrDefaultAsync(d => d.ModelId == modelId && d.Size == size && d.IsActive == true);
        }
        public async Task<int> GetPriceById(int id)
        {
            return await _eventDressRentalContext.Dresses
                .Where(d => d.Id == id)
                .Select(d => d.Price)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Dress>> GetDressesByModelId(int modelId)
        {
            return await _eventDressRentalContext.Dresses
                .Include(d => d.Model)
                .Where(d => d.IsActive == true && d.ModelId == modelId)
                .ToListAsync();
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
             Dress? dress= await _eventDressRentalContext.Dresses
                .FirstOrDefaultAsync(d => d.Id == id);
            dress.IsActive = false;
            await _eventDressRentalContext.SaveChangesAsync();
        }
    }
}
