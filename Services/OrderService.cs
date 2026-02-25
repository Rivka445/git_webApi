using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;
        private readonly IDressService _dressService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IUserService userService, IDressService dressService, ILogger<OrderService> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _userService = userService;
            _dressService = dressService;
            _logger = logger;
        }
        public async Task<bool> IsExistsOrderById(int id)
        {
            return await _orderRepository.IsExistsOrderById(id);
        }
        public async Task<bool> checkOrderItems(NewOrderDTO newOrder)
        {
            Order postOrder = _mapper.Map<NewOrderDTO, Order>(newOrder);
            foreach (var item in postOrder.OrderItems)
            {
                if (await _dressService.GetDressById(item.DressId) == null)
                {
                    _logger.LogWarning("checkOrderItems failed: dress {DressId} not found for user {UserId}", item.DressId, postOrder.UserId);
                    return false;
                }
                bool isValid = await _dressService.IsDressAvailable(item.DressId, postOrder.EventDate);
                if (!isValid)
                {
                    _logger.LogWarning("checkOrderItems failed: dress {DressId} unavailable for date {EventDate}", item.DressId, postOrder.EventDate);
                    return false;
                }
            }
            _logger.LogInformation("checkOrderItems passed for user {UserId} with {ItemCount} items", postOrder.UserId, postOrder.OrderItems.Count);
            return true;   
        }
        public async Task<bool> checkOrderItems(OrderDTO newOrder)
        {
            Order postOrder = _mapper.Map<OrderDTO, Order>(newOrder);
            foreach (var item in postOrder.OrderItems)
            {
                if (await _dressService.GetDressById(item.DressId) == null)
                {
                    _logger.LogWarning("checkOrderItems (OrderDTO) failed: dress {DressId} not found for order {OrderId}", item.DressId, postOrder.Id);
                    return false;
                }
            }
            _logger.LogInformation("checkOrderItems (OrderDTO) passed for order {OrderId} with {ItemCount} items", postOrder.Id, postOrder.OrderItems.Count);
            return true;
        }
        public bool checkStatus(int status)
        {
            var isValid = status >= 1 && status <= 4;
            if (!isValid)
            {
                _logger.LogWarning("checkStatus failed: status {Status} is out of range", status);
            }
            return isValid;
        }
        public bool checkDate(DateOnly date)
        {
            var isValid = date > DateOnly.FromDateTime(DateTime.Now);
            if (!isValid)
            {
                _logger.LogWarning("checkDate failed: date {Date} is not in the future", date);
            }
            return isValid;
        }
        public bool checkDate(DateOnly orderDate, DateOnly eventDate)
        {
            var isValid = orderDate >= DateOnly.FromDateTime(DateTime.Now) && eventDate >= DateOnly.FromDateTime(DateTime.Now);
            if (!isValid)
            {
                _logger.LogWarning("checkDate failed: orderDate {OrderDate}, eventDate {EventDate}", orderDate, eventDate);
            }
            return isValid;
        }
        public async Task<bool> checkPrice(NewOrderDTO order)
        {
            Order postOrder = _mapper.Map<NewOrderDTO, Order>(order);
            int sum = 0;
            foreach (var item in postOrder.OrderItems)
            {
                int dressSum  = await _dressService.GetPriceById(item.DressId);
                sum += dressSum;
            }
            if (sum != postOrder.FinalPrice)
            {
                _logger.LogWarning("checkPrice failed: expected {Expected} calculated {Calculated} for user {UserId}", postOrder.FinalPrice, sum, postOrder.UserId);
                return false;
            }
            _logger.LogInformation("checkPrice passed: total {Total} for user {UserId}", sum, postOrder.UserId);
            return true;
        }
        public async Task <bool> checkPrice(OrderDTO order)
        {
            Order postOrder = _mapper.Map<OrderDTO, Order>(order);
            int sum = 0;
            foreach (var item in postOrder.OrderItems)
            {
                int dressSum = await _dressService.GetPriceById(item.DressId);
                sum += dressSum;
            }
            if (sum != postOrder.FinalPrice)
            {
                _logger.LogWarning("checkPrice (OrderDTO) failed: expected {Expected} calculated {Calculated} for order {OrderId}", postOrder.FinalPrice, sum, postOrder.Id);
                return false;
            }
            _logger.LogInformation("checkPrice (OrderDTO) passed: total {Total} for order {OrderId}", sum, postOrder.Id);
            return true;
        }
        public async Task<OrderDTO> GetOrderById(int id)
        {
            Order? order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return null;
            OrderDTO orderDTO = _mapper.Map<Order, OrderDTO>(order);
            return orderDTO;
        }
        public async Task<List<OrderDTO>> GetAllOrders()
        {
            List<Order> orders = await _orderRepository.GetAllOrders();
            List<OrderDTO> ordersDTO = _mapper.Map<List<Order>, List<OrderDTO>>(orders);
            return ordersDTO;
        }
        public async Task<List<OrderDTO>> GetOrderByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrderByUserId(userId);
            List<OrderDTO> ordersDTO = _mapper.Map<List<Order>, List<OrderDTO>>(orders);
            return ordersDTO;
        }
        public async Task<List<OrderDTO>> GetOrdersByDate(DateOnly date)
        {
            List<Order> orders = await _orderRepository.GetOrdersByDate(date);
            List<OrderDTO> ordersDTO = _mapper.Map<List<Order>, List<OrderDTO>>(orders);
            return ordersDTO;
        }
        public async Task<OrderDTO> AddOrder(NewOrderDTO newOrder)
        {
            Order postOrder = _mapper.Map<NewOrderDTO, Order>(newOrder);
            postOrder.StatusId = 1;
            Order order = await _orderRepository.AddOrder(postOrder);
            OrderDTO orderDTO = _mapper.Map<Order, OrderDTO>(order);
            return orderDTO;
        }
        public async Task UpdateStatusOrder(OrderDTO orderDto, int statusId)
        {
            Order order = _mapper.Map<OrderDTO, Order>(orderDto);
            order.StatusId = statusId;
            await _orderRepository.UpdateStatusOrder(order);
        }
    }
}
