//using Moq;
//using Xunit;
//using Services;
//using Repositories;
//using Entities;
//using DTOs;
//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Services.Tests
//{
//    public class OrderServiceTests
//    {
//        private readonly Mock<IOrderRepository> _orderRepoMock;
//        private readonly Mock<IUserService> _userServiceMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly OrderService _orderService;

//        public OrderServiceTests()
//        {
//            _orderRepoMock = new Mock<IOrderRepository>();
//            _userServiceMock = new Mock<IUserService>();
//            _mapperMock = new Mock<IMapper>();

//            _orderService = new OrderService(
//                _orderRepoMock.Object,
//                _mapperMock.Object,
//                _userServiceMock.Object
//            );
//        }

//        #region GetOrderById Tests
//        [Fact]
//        public async Task GetOrderById_ExistingId_ReturnsOrderDTO()
//        {
//            int orderId = 1;
//            var order = new Order { Id = orderId, FinalPrice = 500 };
//            var orderDto = new OrderDTO(orderId, DateOnly.FromDateTime(DateTime.Now),
//                                        DateOnly.FromDateTime(DateTime.Now).AddDays(7), 500,
//                                        1, "Note", "Status", "First", "Last", new List<DressDTO>());

//            _orderRepoMock.Setup(r => r.GetOrderById(orderId)).ReturnsAsync(order);
//            _mapperMock.Setup(m => m.Map<Order, OrderDTO>(order)).Returns(orderDto);

//            var result = await _orderService.GetOrderById(orderId);

//            Assert.NotNull(result);
//            Assert.Equal(orderId, result.Id);
//        }

//        [Fact]
//        public async Task GetOrderById_InvalidId_ThrowsKeyNotFoundException()
//        {
//            // Arrange
//            _orderRepoMock.Setup(r => r.GetOrderById(0)).ReturnsAsync((Order)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderById(0));
//        }

//        [Fact]
//        public async Task GetOrderById_NonExistingId_ThrowsKeyNotFoundException()
//        {
//            _orderRepoMock.Setup(r => r.GetOrderById(It.IsAny<int>())).ReturnsAsync((Order)null);

//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderById(999));
//        }
//        #endregion

//        #region GetOrderByUserId Tests
//        [Fact]
//        public async Task GetOrderByUserId_UserNotFound_ThrowsKeyNotFoundException()
//        {
//            int userId = 1;
//            _userServiceMock.Setup(s => s.GetUserById(userId)).ReturnsAsync((UserDTO)null);

//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderByUserId(userId));
//        }

//        [Fact]
//        public async Task GetOrderByUserId_NoOrdersFound_ThrowsInvalidOperationException()
//        {
//            int userId = 1;
//            _userServiceMock.Setup(s => s.GetUserById(userId)).ReturnsAsync(new UserDTO(userId, "F", "L", "e@e.com", "050", "Pass"));
//            _orderRepoMock.Setup(r => r.GetOrderByUserId(userId)).ReturnsAsync(new List<Order>());

//            await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.GetOrderByUserId(userId));
//        }

//        [Fact]
//        public async Task GetOrderByUserId_ValidUser_ReturnsOrders()
//        {
//            int userId = 1;
//            var orders = new List<Order> { new Order { Id = 10, FinalPrice = 200 } };
//            var ordersDto = new List<OrderDTO> { new OrderDTO(10, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(1), 200, 1, "Note", "New", "F", "L", new List<DressDTO>()) };

//            _userServiceMock.Setup(s => s.GetUserById(userId)).ReturnsAsync(new UserDTO(userId, "F", "L", "e@e.com", "050", "Pass"));
//            _orderRepoMock.Setup(r => r.GetOrderByUserId(userId)).ReturnsAsync(orders);
//            _mapperMock.Setup(m => m.Map<List<Order>, List<OrderDTO>>(orders)).Returns(ordersDto);

//            var result = await _orderService.GetOrderByUserId(userId);

//            Assert.Single(result);
//            Assert.Equal(10, result[0].Id);
//        }
//        #endregion

//        #region GetOrdersByDate Tests
//        [Fact]
//        public async Task GetOrdersByDate_PastDate_ThrowsArgumentException()
//        {
//            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

//            await Assert.ThrowsAsync<ArgumentException>(() => _orderService.GetOrdersByDate(pastDate));
//        }

//        [Fact]
//        public async Task GetOrdersByDate_ValidDate_ReturnsOrders()
//        {
//            var date = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
//            var orders = new List<Order> { new Order { Id = 5, FinalPrice = 300 } };
//            var ordersDto = new List<OrderDTO> { new OrderDTO(5, date, date.AddDays(1), 300, 1, "Note", "New", "F", "L", new List<DressDTO>()) };

//            _orderRepoMock.Setup(r => r.GetOrdersByDate(date)).ReturnsAsync(orders);
//            _mapperMock.Setup(m => m.Map<List<Order>, List<OrderDTO>>(orders)).Returns(ordersDto);

//            var result = await _orderService.GetOrdersByDate(date);

//            Assert.Single(result);
//            Assert.Equal(5, result[0].Id);
//        }
//        #endregion

//        #region AddOrder Tests
//        [Fact]
//        public async Task AddOrder_NullOrder_ThrowsArgumentNullException()
//        {
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _orderService.AddOrder(null));
//        }

//        [Fact]
//        public async Task AddOrder_PriceMismatch_ThrowsInvalidOperationException()
//        {
//            var items = new List<OrderItemDTO> { new OrderItemDTO(1, 100) };
//            var newOrderDto = new NewOrderDTO(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(2), 500, 1, "Note", items);

//            var postOrder = new Order
//            {
//                FinalPrice = 500,
//                OrderItems = new List<OrderItem> { new OrderItem { Dress = new Dress { Price = 100 } } }
//            };

//            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(newOrderDto)).Returns(postOrder);

//            await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.AddOrder(newOrderDto));
//        }

//        [Fact]
//        public async Task AddOrder_InvalidDate_ThrowsArgumentException()
//        {
//            var pastDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
//            var newOrderDto = new NewOrderDTO(pastDate, pastDate, 100, 1, "Note", new List<OrderItemDTO>());

//            var postOrder = new Order
//            {
//                OrderDate = pastDate,
//                EventDate = pastDate,
//                FinalPrice = 100,
//                OrderItems = new List<OrderItem> {
//                                                 new OrderItem { Dress = new Dress { Price = 100 } }}
//            };
//            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(newOrderDto)).Returns(postOrder);

//            await Assert.ThrowsAsync<ArgumentException>(() => _orderService.AddOrder(newOrderDto));
//        }

//        [Fact]
//        public async Task AddOrder_ValidOrder_ReturnsOrderDTO()
//        {
//            var today = DateOnly.FromDateTime(DateTime.Now);
//            var itemsDto = new List<OrderItemDTO> { new OrderItemDTO(1, 200) };
//            var newOrderDto = new NewOrderDTO(today, today.AddDays(1), 200, 1, "Note", itemsDto);

//            var postOrder = new Order
//            {
//                OrderDate = today,
//                EventDate = today.AddDays(1),
//                FinalPrice = 200,
//                OrderItems = new List<OrderItem> { new OrderItem { Dress = new Dress { Price = 200 } } }
//            };

//            var savedOrder = new Order { Id = 10, FinalPrice = 200 };
//            var expectedDto = new OrderDTO(10, today, today.AddDays(1), 200, 1, "Note", "New", "F", "L", new List<DressDTO>());

//            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(newOrderDto)).Returns(postOrder);
//            _orderRepoMock.Setup(r => r.AddOrder(postOrder)).ReturnsAsync(savedOrder);
//            _mapperMock.Setup(m => m.Map<Order, OrderDTO>(savedOrder)).Returns(expectedDto);

//            var result = await _orderService.AddOrder(newOrderDto);

//            Assert.NotNull(result);
//            Assert.Equal(10, result.Id);
//            _orderRepoMock.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Once);
//        }
//        #endregion

//        #region UpdateStatusOrder Tests
//        [Fact]
//        public async Task UpdateStatusOrder_InvalidStatusId_ThrowsArgumentOutOfRangeException()
//        {
//            var order = new Order { Id = 1 };

//            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _orderService.UpdateStatusOrder(order, 5));
//        }

//        [Fact]
//        public async Task UpdateStatusOrder_ValidStatus_UpdatesOrder()
//        {
//            var order = new Order { Id = 1, StatusId = 1 };
//            int newStatus = 2;

//            await _orderService.UpdateStatusOrder(order, newStatus);

//            Assert.Equal(newStatus, order.StatusId);
//            _orderRepoMock.Verify(r => r.UpdateStatusOrder(order), Times.Once);
//        }
//        #endregion
//    }
//}
