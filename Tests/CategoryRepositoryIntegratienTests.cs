using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CategoryRepositoryIntegratienTests: IClassFixture<DatabaseFixture>
    {
        private readonly WebApiShopContext _dbContext;
        private readonly CategoryRepository _categoryRepository;
        public CategoryRepositoryIntegratienTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _categoryRepository = new CategoryRepository(_dbContext);
        }
        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            var category1 = new Category {  Name = "Electronics" };
            var category2 = new Category {  Name = "Books" };

            _dbContext.Categories.Add(category1);
            _dbContext.Categories.Add(category2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Electronics");
            Assert.Contains(result, c => c.Name == "Books");
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList()
        {
            // Arrange
            // No categories are added to the database
            // Act
            var result = await _categoryRepository.GetCategories();
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
