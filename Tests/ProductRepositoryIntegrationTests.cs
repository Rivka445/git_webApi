using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ProductRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly WebApiShopContext _dbContext;
        private readonly ProductRepository _productRepository;
        public ProductRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _productRepository = new ProductRepository(_dbContext);
        }
        [Fact]
        public async Task GetProducts()
        {
            // Arrange
            var category = new Category
            {
                Name = "Electronics"
            };

            var product1 = new Product
            {
                Name = "Product 1",
                CategoryId = 1,
                Description = "Description for Product 1",
                Price = 10.0,
                ImgUrl= "a.jpg"
            };

            var product2 = new Product
            {
                Name = "Product 2",
                CategoryId = 1,
                Description = "Description for Product 2",
                Price = 15.0,
                ImgUrl = "a.jpg"
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.Products.AddAsync(product1);
            await _dbContext.Products.AddAsync(product2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetProducts(null, null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == product1.Name);
            Assert.Contains(result, p => p.Name == product2.Name);
        }

        [Fact]
        public async Task GetById_HappyPath()
        {
            // Arrange
            var category = new Category
            {
                Name = "Books"
            };

            var product = new Product
            {
                Name = "Product 3",
                CategoryId =1,
                Description = "Description for Product 3",
                Price = 20.0,
                ImgUrl = "a.png"
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
        }

        [Fact]
        public async Task GetById_UnhappyPath_NotFound()
        {
            // Arrange
            // No product with this ID exists

            // Act
            var result = await _productRepository.GetById(999); // Assuming 999 does not exist

            // Assert
            Assert.Null(result);
        }
    }
}

