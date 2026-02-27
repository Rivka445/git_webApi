using AutoMapper;
using DTOs;
using Entities;
using Moq;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Services.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IDressService> _dressServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;

        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _userServiceMock = new Mock<IUserService>();
            _dressServiceMock = new Mock<IDressService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(
                _orderRepoMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object,
                _dressServiceMock.Object,
                _loggerMock.Object
            );
        }

        #region checkStatus

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(4, true)]
        [InlineData(0, false)]
        [InlineData(5, false)]
        public void CheckStatus_ReturnsExpectedResult(int status, bool expected)
        {
            var result = _orderService.checkStatus(status);
            Assert.Equal(expected, result);
        }

        #endregion

        #region checkDate

        [Fact]
        public void CheckDate_SingleDate_Past_ReturnsFalse()
        {
            var past = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);
            Assert.False(_orderService.checkDate(past));
        }

        [Fact]
        public void CheckDate_SingleDate_Future_ReturnsTrue()
        {
            var future = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
            Assert.True(_orderService.checkDate(future));
        }

        [Fact]
        public void CheckDate_OrderAndEventDate_Valid_ReturnsTrue()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var future = today.AddDays(2);

            Assert.True(_orderService.checkDate(today, future));
        }

        #endregion

        #region checkPrice (NewOrderDTO)

        [Fact]
        public async Task CheckPrice_NewOrderDTO_MatchingSum_ReturnsTrue()
        {
            var itemsDto = new List<NewOrderItemDTO>
            {
                new NewOrderItemDTO(1, 100),
                new NewOrderItemDTO(2, 200)
            };

            var dto = new NewOrderDTO(
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now).AddDays(1),
                300,
                1,
                "note",
                itemsDto
            );

            var mappedOrder = new Order
            {
                FinalPrice = 300,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { DressId = 1 },
                    new OrderItem { DressId = 2 }
                }
            };

            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(dto))
                       .Returns(mappedOrder);

            _dressServiceMock.Setup(d => d.GetPriceById(1)).ReturnsAsync(100);
            _dressServiceMock.Setup(d => d.GetPriceById(2)).ReturnsAsync(200);

            var result = await _orderService.checkPrice(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckPrice_NewOrderDTO_NotMatchingSum_ReturnsFalse()
        {
            var itemsDto = new List<NewOrderItemDTO>
            {
                new NewOrderItemDTO(1, 100)
            };

            var dto = new NewOrderDTO(
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now).AddDays(1),
                500,
                1,
                "note",
                itemsDto
            );

            var mappedOrder = new Order
            {
                FinalPrice = 500,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { DressId = 1 }
                }
            };

            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(dto))
                       .Returns(mappedOrder);

            _dressServiceMock.Setup(d => d.GetPriceById(1)).ReturnsAsync(100);

            var result = await _orderService.checkPrice(dto);

            Assert.False(result);
        }

        #endregion

        #region checkOrderItems (NewOrderDTO)


        [Fact]
        public async Task CheckOrderItems_NewOrderDTO_DressNotExists_ReturnsFalse()
        {
            var dto = new NewOrderDTO(
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now).AddDays(2),
                200,
                1,
                "note",
                new List<NewOrderItemDTO>
                {
                    new NewOrderItemDTO(1,100)
                });

            var mappedOrder = new Order
            {
                EventDate = DateOnly.FromDateTime(DateTime.Now).AddDays(2),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { DressId = 1 }
                }
            };

            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(dto))
                       .Returns(mappedOrder);

            //_dressServiceMock.Setup(d => d.GetDressById(1))
            //                 .ReturnsAsync((Dress?)null);

            var result = await _orderService.checkOrderItems(dto);

            Assert.False(result);
        }

        #endregion

        #region AddOrder

        [Fact]
        public async Task AddOrder_ValidOrder_SetsStatusTo1_AndReturnsDTO()
        {
            var dto = new NewOrderDTO(
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now).AddDays(1),
                100,
                1,
                "note",
                new List<NewOrderItemDTO>());

            var mappedOrder = new Order
            {
                FinalPrice = 100
            };

            var savedOrder = new Order
            {
                Id = 10,
                FinalPrice = 100,
                StatusId = 1
            };

            var expectedDto = new OrderDTO
            {
                Id = 10,
                OrderDate = dto.OrderDate,
                EventDate = dto.EventDate,
                FinalPrice = 100,
                UserId = 1,
                StatusId = 1,
                StatusName = "New",
                UserFirstName = "F",
                UserLastName = "L",
                OrderItems = new List<OrderItemDTO>()
            };

            _mapperMock.Setup(m => m.Map<NewOrderDTO, Order>(dto))
                       .Returns(mappedOrder);

            _orderRepoMock.Setup(r => r.AddOrder(mappedOrder))
                          .ReturnsAsync(savedOrder);

            _mapperMock.Setup(m => m.Map<Order, OrderDTO>(savedOrder))
                       .Returns(expectedDto);

            var result = await _orderService.AddOrder(dto);

            Assert.NotNull(result);
            Assert.Equal(1, mappedOrder.StatusId);
            Assert.Equal(10, result.Id);

            _orderRepoMock.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Once);
        }

        #endregion
    }
}