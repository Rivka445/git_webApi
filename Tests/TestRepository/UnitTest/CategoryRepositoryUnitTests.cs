using Entities;
using Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CategoryRepositoryUnitTests
    {

        #region Get Ctegories
        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            var mockContext = new Mock<EventDressRentalContext>(new DbContextOptions<EventDressRentalContext>());
            var categories = new List<Category>
            {
                new Category { Id = 1 , Name = "Electronics" , Description = "..." },
                new Category { Id = 2 , Name = "Books" , Description = "..." }
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
            var mockContext = new Mock<EventDressRentalContext>(new DbContextOptions<EventDressRentalContext>());
            var categoryRepository = new CategoryRepository(mockContext.Object);
            var categories = new List<Category>();

            mockContext.Setup(ctx => ctx.Categories).ReturnsDbSet(categories);

            // Act
            var result = await categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Add Category
        [Fact]
        public async Task AddCategory_ShouldAddCategoryAndSaveChanges()
        {
            // Arrange
            var mockContext = new Mock<EventDressRentalContext>(new DbContextOptions<EventDressRentalContext>());

            var categories = new List<Category>();
            mockContext.Setup(ctx => ctx.Categories).ReturnsDbSet(categories);

            var repository = new CategoryRepository(mockContext.Object);
            var newCategory = new Category { Id = 1, Name = "שמלות כלה" };

            // Act
            var result = await repository.AddCategory(newCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("שמלות כלה", result.Name);

            mockContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
