using Entities;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
namespace Tests
{
    public class ProductRepositoryUnitTesting
    {
        
        [Fact]
        public async Task GetProducts_ReturnsProducts()
        {
            // Arrange
            var _mockContext = new Mock<WebApiShopContext>();
            var products = new List<Product>
            {
                new Product {Id=1, Name = "Product1", Price = 100, Description = "Description1", CategoryId = 1 },
                new Product {Id=2, Name = "Product2", Price = 200, Description = "Description2", CategoryId = 2 }
            };

            _mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);
            var _productRepository = new ProductRepository(_mockContext.Object);

            // Act
            var result = await _productRepository.GetProducts(null, null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "Product1");
            Assert.Contains(result, r => r.Name == "Product2");
        }

        [Fact]
        public async Task GetProducts_ReturnsEmptyList()
        {
            // Arrange
            var products = new List<Product>();
            var _mockContext = new Mock<WebApiShopContext>();

            _mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);
            var _productRepository = new ProductRepository(_mockContext.Object);

            // Act
            var result = await _productRepository.GetProducts(null, null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct()
        {
            // Arrange
            var _mockContext = new Mock<WebApiShopContext>();
            var product = new Product { Id = 1, Name = "Product1", Price = 100, Description = "Description1", CategoryId = 1 };
            var products = new List<Product> { product };

            _mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);
            var _productRepository = new ProductRepository(_mockContext.Object);

            // Act
            var result = await _productRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Product1", result.Name);
        }

        [Fact]
        public async Task GetProductById_ReturnsNull()
        {
            // Arrange
            var products = new List<Product>();
            var _mockContext = new Mock<WebApiShopContext>();

            _mockContext.Setup(ctx => ctx.Products).ReturnsDbSet(products);
            var _productRepository = new ProductRepository(_mockContext.Object);

            // Act
            var result = await _productRepository.GetById(999); 

            // Assert
            Assert.Null(result);
        }
    }
}

