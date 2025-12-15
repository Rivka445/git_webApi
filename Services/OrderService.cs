using AutoMapper;
using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository,IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }
        public async Task<OrderDTO> GetOrderById(int id)
        {
            Order order = await _orderRepository.GetById(id);
            OrderDTO orderDTO= _mapper.Map<Order,OrderDTO>(order);
            return orderDTO;
        }
        public async Task<OrderDTO> AddOrder(Order newOrder)
        {
            Order order= await _orderRepository.addOrder(newOrder);
            OrderDTO orderDTO = _mapper.Map<Order, OrderDTO>(order);
            return orderDTO;
        }
    }
}
