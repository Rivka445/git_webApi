using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tests
{
    public class OrderRepositoryIntegrationTests: IClassFixture<DatabaseFixture>
    {
        private readonly WebApiShopContext _dbContext;
        private readonly OrderRepository _orderRepository;
        public OrderRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _orderRepository = new OrderRepository(_dbContext);
        }
        [Fact]
        public async Task AddOrder_HappyPath()
        {
            // Arrange
            var category = new Category
            {
                Name = "Electronics"
            };

            var user = new User
            {
                Email = "testuser@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password123"
            };

            var product1 = new Product
            {
                Name = "Product 1",
                CategoryId = 1,
                Description = "Description for Product 1",
                Price = 10.0,
                ImgUrl="a.png"
            };

            var product2 = new Product
            {
                Name = "Product 2",
                CategoryId =1,
                Description = "Description for Product 2",
                Price = 15.0,
                ImgUrl = "a.png"
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.Products.AddAsync(product1);
            await _dbContext.Products.AddAsync(product2);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Sum = 35, // 2 * 10 + 1 * 15
                UserId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId =1, Quantity = 2 },
                    new OrderItem { ProductId = 2, Quantity = 1 }
                }
            };

            // Act
            var result = await _orderRepository.addOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Sum, result.Sum);
            Assert.Equal(2, result.OrderItems.Count); // Verify total items
        }

        [Fact]
        public async Task GetById_ReturnsOrder()
        {
            // Arrange
            var category = new Category
            {
                Name = "Books"
            };

            var user = new User
            {
                Email = "testuser2@example.com",
                FirstName = "Test2",
                LastName = "User2",
                Password = "password456"
            };

            var product = new Product
            {
                Name = "Product 3",
                CategoryId = 1,
                Description = "Description for Product 3",
                Price = 20.0,
                ImgUrl = "a.png"
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                Sum = 20, // 1 * 20
                UserId =1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1 }
                }
            };

            await _orderRepository.addOrder(order);

            // Act
            var result = await _orderRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Sum, result.Sum);
            Assert.Single(result.OrderItems);
        }

        [Fact]
        public async Task GetById_ReturnsNull_UnhappyPath()
        {
            // Arrange
            // No order with this ID exists

            // Act
            var result = await _orderRepository.GetById(999); // Assuming 999 does not exist

            // Assert
            Assert.Null(result);
        }
    }
}

