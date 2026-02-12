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
    public class ModelRepositoryUnitTests
    {
        private Mock<EventDressRentalContext> GetMockContext()
        {
            var options = new DbContextOptionsBuilder<EventDressRentalContext>().Options;
            return new Mock<EventDressRentalContext>(options);
        }

        #region GetModelById

        [Fact]
        public async Task GetModelById_ReturnsActiveModel()
        {
            var mockContext = GetMockContext();

            var models = new List<Model>
            {
                new Model { Id = 1, IsActive = true },
                new Model { Id = 2, IsActive = false }
            };

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var result = await repository.GetModelById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetModelById_ReturnsNull_WhenInactiveOrNotExists()
        {
            var mockContext = GetMockContext();

            var models = new List<Model>
            {
                new Model { Id = 1, IsActive = false }
            };

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var result1 = await repository.GetModelById(1);
            var result2 = await repository.GetModelById(99);

            Assert.Null(result1);
            Assert.Null(result2);
        }

        #endregion

        #region GetModels - Filtering

        [Fact]
        public async Task GetModels_FiltersByDescription()
        {
            var mockContext = GetMockContext();

            var models = new List<Model>
            {
                new Model { Id = 1, Description = "Red Dress Elegant", BasePrice = 100, IsActive = true },
                new Model { Id = 2, Description = "Blue Dress", BasePrice = 200, IsActive = true }
            };

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var (items, total) = await repository.GetModels("Red", null, null, new int[] { }, null);

            Assert.Single(items);
            Assert.Equal(1, total);
            Assert.Contains("Red", items.First().Description);
        }

        [Fact]
        public async Task GetModels_FiltersByPriceRange()
        {
            var mockContext = GetMockContext();

            var models = new List<Model>
            {
                new Model { Id = 1, BasePrice = 100, IsActive = true },
                new Model { Id = 2, BasePrice = 300, IsActive = true },
                new Model { Id = 3, BasePrice = 700, IsActive = true }
            };

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var (items, total) = await repository.GetModels(null, 200, 600, new int[] { }, null);

            Assert.Single(items);
            Assert.Equal(1, total);
            Assert.Equal(300, items.First().BasePrice);
        }

        [Fact]
        public async Task GetModels_FiltersByColor()
        {
            var mockContext = GetMockContext();

            var models = new List<Model>
            {
                new Model { Id = 1, Color = "Red", IsActive = true },
                new Model { Id = 2, Color = "Blue", IsActive = true }
            };

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var (items, total) = await repository.GetModels(null, null, null, new int[] { }, "Red");

            Assert.Single(items);
            Assert.Equal(1, total);
            Assert.Equal("Red", items.First().Color);
        }

        [Fact]
        public async Task GetModels_FiltersByCategories()
        {
            var mockContext = GetMockContext();

            var category1 = new Category { Id = 1 };
            var category2 = new Category { Id = 2 };

            var models = new List<Model>
            {
                new Model { Id = 1, IsActive = true, Categories = new List<Category> { category1 } },
                new Model { Id = 2, IsActive = true, Categories = new List<Category> { category2 } }
            };

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var (items, total) = await repository.GetModels(null, null, null, new int[] { 1 }, null);

            Assert.Single(items);
            Assert.Equal(1, total);
            Assert.Equal(1, items.First().Id);
        }

        #endregion

        #region Pagination

        [Fact]
        public async Task GetModels_AppliesPaginationCorrectly()
        {
            var mockContext = GetMockContext();

            var models = Enumerable.Range(1, 20)
                .Select(i => new Model
                {
                    Id = i,
                    BasePrice = i * 10,
                    IsActive = true
                }).ToList();

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);

            var repository = new ModelRepository(mockContext.Object);

            var (items, total) = await repository.GetModels(null, null, null, new int[] { }, null, position: 2, skip: 5);

            Assert.Equal(20, total);
            Assert.Equal(5, items.Count);
            Assert.Equal(6, items.First().Id);
        }

        #endregion

        #region Add / Update / Delete

        [Fact]
        public async Task AddModel_CallsSaveChangesOnce()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Models).ReturnsDbSet(new List<Model>());

            var repository = new ModelRepository(mockContext.Object);

            var model = new Model { Id = 10 };

            var result = await repository.AddModel(model);

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task UpdateModel_CallsUpdateAndSave()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Models).ReturnsDbSet(new List<Model>());

            var repository = new ModelRepository(mockContext.Object);

            var model = new Model { Id = 1 };

            await repository.UpdateModel(model);

            mockContext.Verify(x => x.Models.Update(model), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteModel_CallsUpdateAndSave()
        {
            var mockContext = GetMockContext();
            mockContext.Setup(x => x.Models).ReturnsDbSet(new List<Model>());

            var repository = new ModelRepository(mockContext.Object);

            var model = new Model { Id = 1, IsActive = false };

            await repository.DeleteModel(model);

            mockContext.Verify(x => x.Models.Update(model), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
