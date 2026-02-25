using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;
using Xunit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestProject
{
    [Collection("Database Collection")]
    public class CategoryRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly EventDressRentalContext _dbContext;
        private readonly CategoryRepository _categoryRepository;

        public CategoryRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.Context;
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        public async Task InitializeAsync()
        {
            await ClearDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await ClearDatabaseAsync();
        }

        private async Task ClearDatabaseAsync()
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.OrderItems.RemoveRange(_dbContext.OrderItems);
            _dbContext.Orders.RemoveRange(_dbContext.Orders);
            _dbContext.Dresses.RemoveRange(_dbContext.Dresses);
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Models_Categories]");
            _dbContext.Models.RemoveRange(_dbContext.Models);
            _dbContext.Categories.RemoveRange(_dbContext.Categories);
            _dbContext.Statuses.RemoveRange(_dbContext.Statuses);
            _dbContext.Users.RemoveRange(_dbContext.Users);
            await _dbContext.SaveChangesAsync();
        }

        #region GetCategories Tests

        [Fact]
        public async Task GetCategories_ReturnsEmpty_WhenNoDataExists()
        {
            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCategories_WhenDataExists_ReturnsAllCategories()
        {
            // Arrange
            var testCategories = new List<Category>
            {
                new Category { Name = "חסידי" },
                new Category { Name = "ליטאי" }
            };
            await _dbContext.Categories.AddRangeAsync(testCategories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "חסידי");
        }

        #endregion

        #region GetCategoryById & Existence Tests

        [Fact]
        public async Task GetCategoryById_ReturnsCorrectCategory_WhenExists()
        {
            // Arrange
            var category = new Category { Name = "ערב" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategoryById(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ערב", result!.Name);
        }

        [Fact]
        public async Task IsExistsCategoryById_ReturnsTrue_WhenCategoryExists()
        {
            // Arrange
            var category = new Category { Name = "ילדות" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var exists = await _categoryRepository.IsExistsCategoryById(category.Id);
            var notExists = await _categoryRepository.IsExistsCategoryById(999);

            // Assert
            Assert.True(exists);
            Assert.False(notExists);
        }

        #endregion

        #region AddCategory Tests

        [Fact]
        public async Task AddCategory_SavesCategoryToDatabase()
        {
            // Arrange
            var newCategory = new Category { Name = "חדש מהניילון" };

            // Act
            var result = await _categoryRepository.AddCategory(newCategory);

            // Assert
            var categoryInDb = await _dbContext.Categories.FindAsync(result.Id);
            Assert.NotNull(categoryInDb);
            Assert.Equal("חדש מהניילון", categoryInDb!.Name);
        }

        #endregion
    }
}