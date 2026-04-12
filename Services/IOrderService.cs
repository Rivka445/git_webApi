using DTOs;

namespace Services
{
    public interface IOrderService
    {
        Task<bool> IsExistsOrderById(int id);
        Task<OrderDTO> AddOrder(NewOrderDTO newOrder);
        Task<bool> CheckOrderItems(NewOrderDTO newOrder);
        bool CheckDate(DateOnly date);
        bool CheckDate(DateOnly orderDate, DateOnly eventDate);
        Task<bool> CheckPrice(NewOrderDTO order);
        bool CheckStatus(int status);
        Task<List<OrderDTO>> GetAllOrders();
        Task<OrderDTO> GetOrderById(int id);
        Task<List<OrderDTO>> GetOrderByUserId(int userId);
        Task<List<OrderDTO>> GetOrdersByDate(DateOnly date);
        Task UpdateStatusOrder(OrderDTO orderDto, int statusId);
    }
}