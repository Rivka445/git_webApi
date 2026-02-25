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
    public class DressRepositoryUnitTests
    {
        private Mock<EventDressRentalContext> GetMockContext()
        {
            var options = new DbContextOptionsBuilder<EventDressRentalContext>().Options;
            return new Mock<EventDressRentalContext>(options);
        }

        #region Getters & Existence

        [Fact]
        public async Task GetSizesByModelId_ReturnsDistinctActiveSizes()
        {
            var mockContext = GetMockContext();
            var dresses = new List<Dress>
            {
                new Dress { ModelId = 1, Size = "S", IsActive = true },
                new Dress { ModelId = 1, Size = "S", IsActive = true }, // כפילות
                new Dress { ModelId = 1, Size = "M", IsActive = true },
                new Dress { ModelId = 1, Size = "L", IsActive = false }, // לא פעיל
                new Dress { ModelId = 2, Size = "XL", IsActive = true }  // מודל אחר
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);
            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetSizesByModelId(1);

            Assert.Equal(2, result.Count);
            Assert.Contains("S", result);
            Assert.Contains("M", result);
            Assert.DoesNotContain("L", result);
        }

        [Fact]
        public async Task GetDressesByModelId_ReturnsOnlyActiveForModel()
        {
            var mockContext = GetMockContext();
            var dresses = new List<Dress>
            {
                new Dress { Id = 1, ModelId = 10, IsActive = true, Model = new Model() },
                new Dress { Id = 2, ModelId = 10, IsActive = false, Model = new Model() },
                new Dress { Id = 3, ModelId = 11, IsActive = true, Model = new Model() }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);
            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetDressesByModelId(10);

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        #endregion

        #region Availability Logic (The 7-Day Rule)

        [Fact]
        public async Task IsDressAvailable_ReturnsTrue_WhenNoOrdersIn7DayRange()
        {
            var mockContext = GetMockContext();
            var targetDate = new DateOnly(2025, 5, 10);

            var dresses = new List<Dress>
            {
                new Dress
                {
                    Id = 1, IsActive = true,
                    OrderItems = new List<OrderItem>
                    { 
                        // הזמנה רחוקה (מעל 7 ימים) - לא אמורה להפריע
                        new OrderItem { Order = new Order { EventDate = targetDate.AddDays(10) } }
                    }
                }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);
            var repository = new DressRepository(mockContext.Object);

            var result = await repository.IsDressAvailable(1, targetDate);

            Assert.True(result);
        }

        [Fact]
        public async Task GetCountByModelIdAndSizeForDate_ReturnsZero_IfAllDressesBooked()
        {
            var mockContext = GetMockContext();
            var targetDate = new DateOnly(2025, 6, 1);

            var dresses = new List<Dress>
            {
                new Dress
                {
                    Id = 1, ModelId = 1, Size = "S", IsActive = true,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Order = new Order { EventDate = targetDate.AddDays(-3) } }
                    }
                }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);
            var repository = new DressRepository(mockContext.Object);

            var count = await repository.GetCountByModelIdAndSizeForDate(1, "S", targetDate);

            Assert.Equal(0, count);
        }

        #endregion

        #region Write Operations

        [Fact]
        public async Task AddDress_VerifiesDatabaseCalls()
        {
            var mockContext = GetMockContext();
            var dressToAdd = new Dress { Id = 50, ModelId = 1 };

            // בגלל שב-Repository יש FirstAsync אחרי ההוספה, אנחנו חייבים שהרשימה תכיל את האיבר
            var dresses = new List<Dress> { dressToAdd };
            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);

            var repository = new DressRepository(mockContext.Object);

            var result = await repository.AddDress(dressToAdd);

            mockContext.Verify(x => x.Dresses.AddAsync(dressToAdd, It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.Equal(50, result.Id);
        }

        #endregion
    }
}