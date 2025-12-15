using Entities;
using DTOs;
namespace Services
{
    public interface IOrderService
    {
        Task<OrderDTO> AddOrder(Order order);
        Task<OrderDTO> GetOrderById(int id);
    }
}