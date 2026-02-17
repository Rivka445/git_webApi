using DTOs;

namespace Services
{
    public interface IOrderService
    {
        Task<bool> IsExistsOrderById(int id);
        Task<OrderDTO> AddOrder(NewOrderDTO newOrder);
        Task<bool> checkOrderItems(NewOrderDTO newOrder);
        Task<bool> checkOrderItems(OrderDTO newOrder);
        bool checkDate(DateOnly date);
        bool checkDate(DateOnly orderDate, DateOnly eventDate);
        Task<bool> checkPrice(NewOrderDTO order);
        Task<bool> checkPrice(OrderDTO order);
        bool checkStatus(int status);
        Task<List<OrderDTO>> GetAllOrders();
        Task<OrderDTO> GetOrderById(int id);
        Task<List<OrderDTO>> GetOrderByUserId(int userId);
        Task<List<OrderDTO>> GetOrdersByDate(DateOnly date);
        Task UpdateOrder(OrderDTO orderDto, int id);
        Task UpdateStatusOrder(OrderDTO orderDto, int statusId);
    }
}