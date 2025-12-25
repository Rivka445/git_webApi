using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;

namespace Tests
{
    public class OrderRepositoryUnitTesting
    {
        [Fact]
        public async Task AddOrder_ReturnsOrder()
        {
            // Arrange
            var _mockContext = new Mock<WebApiShopContext>();
            var orders = new List<Order>
            {
               new Order { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), Sum = 250 }
            };
            var order = new Order { Id = 2, Date = DateOnly.FromDateTime(DateTime.Now), Sum = 200 };

            _mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);
            var orderRepository = new OrderRepository(_mockContext.Object);

            // Act
            var result = await orderRepository.addOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal(200, result.Sum);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder()
        {
            // Arrange
            var order = new Order { Id = 1, Date = DateOnly.FromDateTime(DateTime.Now), Sum = 250 };
            var orders = new List<Order> { order };

            var _mockContext = new Mock<WebApiShopContext>();
            _mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);
            var orderRepository = new OrderRepository(_mockContext.Object);

            // Act
            var result = await orderRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(250, result.Sum);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull()
        {
            // Arrange
            var orders = new List<Order>();

            var _mockContext = new Mock<WebApiShopContext>();
            _mockContext.Setup(ctx => ctx.Orders).ReturnsDbSet(orders);
            var orderRepository = new OrderRepository(_mockContext.Object);

            // Act
            var result = await orderRepository.GetById(999); // Assuming 999 does not exist.

            // Assert
            Assert.Null(result);
        }
    }
}