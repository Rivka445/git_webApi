using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Data.SqlClient;
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

        public OrderService(IOrderRepository orderRepository, IMapper mapper, IUserService userService, IDressService dressService)
        {
            _userService = userService;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _userService = userService;
            _dressService = dressService;
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
                    return false;
                }
                bool isValid = await _dressService.IsDressAvailable(item.DressId, postOrder.EventDate);
                if (!isValid)
                      return false;
            }
            return true;   
        }
        public async Task<bool> checkOrderItems(OrderDTO newOrder)
        {
            Order postOrder = _mapper.Map<OrderDTO, Order>(newOrder);
            foreach (var item in postOrder.OrderItems)
            {
                if (await _dressService.GetDressById(item.DressId) == null)
                {
                    return false;
                }
            }
            return true;
        }
        public bool checkStatus(int status)
        {
            return (status >= 1 && status <= 4);
        }
        public bool checkDate(DateOnly date)
        {
            return date > DateOnly.FromDateTime(DateTime.Now);
        }
        public bool checkDate(DateOnly orderDate, DateOnly eventDate)
        {
            return orderDate >= DateOnly.FromDateTime(DateTime.Now) && eventDate >= DateOnly.FromDateTime(DateTime.Now);
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
                return false;
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
                return false;
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
        public async Task UpdateOrder(NewOrderDTO orderDto, int id)
        {
            Order order = _mapper.Map<NewOrderDTO, Order>(orderDto);
            await _orderRepository.UpdateOrder(order);
        }
        public async Task UpdateStatusOrder(OrderDTO orderDto, int statusId)
        {
            Order order = _mapper.Map<OrderDTO, Order>(orderDto);
            order.StatusId = statusId;
            await _orderRepository.UpdateStatusOrder(order);
        }
    }
}
