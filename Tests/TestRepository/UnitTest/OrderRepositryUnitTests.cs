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

        #region Get & Existence

        [Fact]
        public async Task IsExistsOrderById_ReturnsTrue_WhenOrderExists()
        {
            var mockContext = GetMockContext();
            var orders = new List<Order> { new Order { Id = 1 } };
            mockContext.Setup(x => x.Orders).ReturnsDbSet(orders);

            var repository = new OrderRepository(mockContext.Object);
            var result = await repository.IsExistsOrderById(1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetOrderById_IncludesNestedEntities()
        {
            var mockContext = GetMockContext();
            var order = new Order
            {
                Id = 1,
                User = new User { FirstName = "Client A" , LastName="",Phone="08",Password="2DSCC2DS"},
                Status = new Status { Name = "Confirmed" },
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Dress = new Dress { Model = new Model { Name = "Bridal" } } }
                }
            };

            mockContext.Setup(x => x.Orders).ReturnsDbSet(new List<Order> { order });
            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderById(1);

            Assert.NotNull(result);
            Assert.Equal("Client A", result.User.FirstName);
            Assert.Equal("Bridal", result.OrderItems.First().Dress.Model.Name);
        }

        #endregion

        #region Specialized Queries

        [Fact]
        public async Task GetOrdersByDate_FiltersCorrectDateAndStatus()
        {
            var mockContext = GetMockContext();
            var targetDate = new DateOnly(2025, 3, 1);

            var orders = new List<Order>
            {
                // מתאים: תאריך אירוע לפני היעד וסטטוס 1
                new Order { Id = 1, EventDate = new DateOnly(2025, 2, 28), StatusId = 1, OrderDate = targetDate },
                // לא מתאים: תאריך אירוע אחרי היעד
                new Order { Id = 2, EventDate = new DateOnly(2025, 3, 5), StatusId = 1, OrderDate = targetDate },
                // לא מתאים: סטטוס לא 1
                new Order { Id = 3, EventDate = new DateOnly(2025, 2, 20), StatusId = 2, OrderDate = targetDate }
            };

            mockContext.Setup(x => x.Orders).ReturnsDbSet(orders);
            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrdersByDate(targetDate);

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetOrderByUserId_OrdersByOrderDateAscending()
        {
            var mockContext = GetMockContext();
            var userId = 10;
            var orders = new List<Order>
            {
                new Order { Id = 1, UserId = userId, OrderDate = new DateOnly(2025, 5, 20) },
                new Order { Id = 2, UserId = userId, OrderDate = new DateOnly(2025, 5, 10) }
            };

            mockContext.Setup(x => x.Orders).ReturnsDbSet(orders);
            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.GetOrderByUserId(userId);

            Assert.Equal(2, result.Count);
            Assert.Equal(2, result[0].Id); // המוקדם יותר אמור להיות ראשון (OrderDate)
        }

        #endregion

        #region Write Operations

        [Fact]
        public async Task AddOrder_SavesAndReturnsFullObject()
        {
            var mockContext = GetMockContext();
            var newOrder = new Order { Id = 1, UserId = 100 };

            // המוקד של FirstOrDefaultAsync בתוך AddOrder מחייב שהאובייקט יהיה ב-Set
            mockContext.Setup(x => x.Orders).ReturnsDbSet(new List<Order> { newOrder });
            var repository = new OrderRepository(mockContext.Object);

            var result = await repository.AddOrder(newOrder);

            mockContext.Verify(x => x.Orders.AddAsync(newOrder, default), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(100, result.UserId);
        }

        #endregion
    }
}