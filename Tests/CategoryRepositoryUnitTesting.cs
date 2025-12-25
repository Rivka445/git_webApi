using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CategoryRepositoryUnitTesting
    {
        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var categories = new List<Category>
            {
                new Category {  Name = "Electronics" },
                new Category {  Name = "Books" }
            };
            mockContext.Setup(ctx => ctx.Categories).ReturnsDbSet(categories);
            var categoryRepository = new CategoryRepository(mockContext.Object);
   
            // Act
            var result = await categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Name == "Electronics");
            Assert.Contains(result, r => r.Name == "Books");
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var categoryRepository = new CategoryRepository(mockContext.Object);
            var categories = new List<Category>();

            mockContext.Setup(ctx => ctx.Categories).ReturnsDbSet(categories);

            // Act
            var result = await categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

}
