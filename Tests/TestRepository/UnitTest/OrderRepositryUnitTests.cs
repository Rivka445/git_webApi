using Entities;
using Moq;
using Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using Xunit;
using System;

namespace Tests
{
    public class OrderRepositoryUnitTests
    {
        private Mock<EventDressRentalContext> GetMockContext()
        {
            var options = new DbContextOptionsBuilder<EventDressRentalContext>().Options;
            return new Mock<EventDressRentalContext>(options);
        }

        #region GetOrderById

        [Fact]
        public async Task GetOrderById_ReturnsOrderWithIncludes()
        {
            var mockContext = GetMockContext();

            var order = new Order
            {
                Id = 10,
                Status = new Status { Id = 1, Name = "Pending" },
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = 1,
                        Dress = new Dress { Id = 5 }
                    }
                }
            };

            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order> { order });

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderById(10);

            Assert.NotNull(result);
            Assert.NotNull(result!.Status);
            Assert.Single(result.OrderItems);
            Assert.NotNull(result.OrderItems.First().Dress);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull_WhenNotExists()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderById(999);

            Assert.Null(result);
        }

        #endregion

        #region GetAllOrders

        [Fact]
        public async Task GetAllOrders_ReturnsSortedByOrderDate()
        {
            var mockContext = GetMockContext();

            var orders = new List<Order>
            {
                new Order { Id = 1, OrderDate = new DateOnly(2024,1,10) },
                new Order { Id = 2, OrderDate = new DateOnly(2024,1,1) }
            };

            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(orders);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetAllOrders();

            Assert.Equal(2, result.Count);
            Assert.True(result[0].OrderDate <= result[1].OrderDate);
        }

        #endregion

        #region GetOrderByUserId

        [Fact]
        public async Task GetOrderByUserId_ReturnsOnlyUserOrdersSorted()
        {
            var mockContext = GetMockContext();

            var userId = 5;

            var orders = new List<Order>
            {
                new Order { Id = 1, UserId = userId, OrderDate = new DateOnly(2024,1,2) },
                new Order { Id = 2, UserId = userId, OrderDate = new DateOnly(2024,1,1) },
                new Order { Id = 3, UserId = 99, OrderDate = new DateOnly(2024,1,3) }
            };

            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(orders);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderByUserId(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, o => Assert.Equal(userId, o.UserId));
            Assert.True(result[0].OrderDate <= result[1].OrderDate);
        }

        [Fact]
        public async Task GetOrderByUserId_ReturnsEmpty_WhenNoOrders()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderByUserId(1);

            Assert.Empty(result);
        }

        #endregion

        #region GetOrdersByDate

        [Fact]
        public async Task GetOrdersByDate_FiltersByEventDateAndStatus()
        {
            var mockContext = GetMockContext();

            var targetDate = new DateOnly(2024, 1, 10);

            var orders = new List<Order>
            {
                new Order { Id = 1, EventDate = targetDate.AddDays(-1), StatusId = 1, OrderDate = targetDate },
                new Order { Id = 2, EventDate = targetDate.AddDays(1), StatusId = 1, OrderDate = targetDate },
                new Order { Id = 3, EventDate = targetDate.AddDays(-2), StatusId = 2, OrderDate = targetDate }
            };

            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(orders);

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrdersByDate(targetDate);

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetOrdersByDate_ReturnsEmpty_WhenNoMatch()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrdersByDate(new DateOnly(2024, 1, 1));

            Assert.Empty(result);
        }

        #endregion

        #region Add / Update

        [Fact]
        public async Task AddOrder_CallsSaveChangesOnce()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            var repository = new OrderRepository(mockContext.Object);

            var order = new Order { UserId = 1 };

            var result = await repository.AddOrder(order);

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.Equal(order.UserId, result.UserId);
        }

        [Fact]
        public async Task UpdateOrder_CallsUpdateAndSave()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            var repository = new OrderRepository(mockContext.Object);

            var order = new Order { Id = 1 };

            await repository.UpdateOrder(order);

            mockContext.Verify(x => x.Orders.Update(order), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusOrder_CallsUpdateAndSave()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Orders)
                .ReturnsDbSet(new List<Order>());

            var repository = new OrderRepository(mockContext.Object);

            var order = new Order { Id = 1, StatusId = 1 };

            order.StatusId = 2;

            await repository.UpdateStatusOrder(order);

            mockContext.Verify(x => x.Orders.Update(order), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
