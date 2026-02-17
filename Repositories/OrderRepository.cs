using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EventDressRentalContext _eventDressRentalContext;
        public OrderRepository(EventDressRentalContext webApiShopContext)
        {
            _eventDressRentalContext = webApiShopContext;
        }
        public async Task<bool> IsExistsOrderById(int id)
        {
            return await _eventDressRentalContext.Orders.AnyAsync(c => c.Id == id);
        }
        public async Task<Order?> GetOrderById(int id)
        {
            return await _eventDressRentalContext.Orders
                            .Include(o => o.OrderItems).ThenInclude(o => o.Dress).ThenInclude(o => o.Model)
                            .Include(o => o.User)
                            .Include(o => o.Status)
                            .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetAllOrders()
        {
            return await _eventDressRentalContext.Orders
                            .Include(o => o.OrderItems).ThenInclude(oi => oi.Dress).ThenInclude(o => o.Model)
                            .Include(o => o.User)
                            .Include(o => o.Status)
                            .OrderBy(o => o.OrderDate)
                            .ToListAsync();
        }
        public async Task<List<Order>> GetOrderByUserId(int id)
        {
            return await _eventDressRentalContext.Orders
                            .Include(o => o.OrderItems).ThenInclude(oi => oi.Dress).ThenInclude(o => o.Model)
                            .Include(o => o.User)
                            .Include(o => o.Status)
                            .Where(o => o.UserId == id)
                            .OrderBy(o => o.OrderDate)
                            .ToListAsync();     
        }
        public async Task<List<Order>> GetOrdersByDate(DateOnly date)
        {
            return await _eventDressRentalContext.Orders
                       .Include(o => o.OrderItems).ThenInclude(oi => oi.Dress).ThenInclude(o => o.Model)
                       .Include(o => o.User)
                       .Include(o => o.Status)
                       .Where(o => o.EventDate <= date && o.StatusId == 1)
                       .OrderBy(o => o.OrderDate)
                       .ToListAsync();
        }
        public async Task<Order> AddOrder(Order order)
        {
            await _eventDressRentalContext.Orders.AddAsync(order);
            await _eventDressRentalContext.SaveChangesAsync();
            return order;
        }
        public async Task UpdateOrder(Order order)
        {
            _eventDressRentalContext.Orders.Update(order);
            await _eventDressRentalContext.SaveChangesAsync();
        }
        public async Task UpdateStatusOrder(Order order)
        {
            await _eventDressRentalContext.Orders
            .Where(d => d.Id == order.Id)
            .ExecuteUpdateAsync(s => s
            .SetProperty(d => d.StatusId, order.StatusId)
            );
        }
    }
}
