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

        #region GetModels - Complex Filtering & Pagination

        [Fact]
        public async Task GetModels_Pagination_ReturnsCorrectSlice()
        {
            var mockContext = GetMockContext();
            var models = Enumerable.Range(1, 15).Select(i => new Model
            {
                Id = i,
                BasePrice = i * 10,
                IsActive = true,
                Name = $"Model {i}",
                Color = "White"
            }).ToList();

            mockContext.Setup(x => x.Models).ReturnsDbSet(models);
            var repository = new ModelRepository(mockContext.Object);

            // עמוד 2, 5 פריטים בעמוד
            var (items, total) = await repository.GetModels(null, null, null, new int[] { }, new string[] { }, position: 2, skip: 5);

            Assert.Equal(5, items.Count);
            Assert.Equal(15, total);
            Assert.Equal(6, items.First().Id); // אחרי ה-5 הראשונים
        }

        #endregion

        #region   Update Logic



        [Fact]
        public async Task UpdateModel_UpdatesPropertiesCorrectly()
        {
            var mockContext = GetMockContext();
            var existingModel = new Model { Id = 1, Name = "Old Name", Categories = new List<Category>() };
            var updatedModel = new Model { Id = 1, Name = "New Name", BasePrice = 999, Categories = new List<Category>() };

            mockContext.Setup(x => x.Models).ReturnsDbSet(new List<Model> { existingModel });
            var repository = new ModelRepository(mockContext.Object);

            await repository.UpdateModel(updatedModel);

            Assert.Equal("New Name", existingModel.Name);
            Assert.Equal(999, existingModel.BasePrice);
            mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        #endregion

    }
}