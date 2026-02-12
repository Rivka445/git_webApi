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

        #region GetDressById

        [Fact]
        public async Task GetDressById_ReturnsActiveDress()
        {
            var mockContext = GetMockContext();

            var dresses = new List<Dress>
            {
                new Dress { Id = 1, IsActive = true },
                new Dress { Id = 2, IsActive = false }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);

            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetDressById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetDressById_ReturnsNull_WhenNotExists()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(new List<Dress>());

            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetDressById(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetDressById_ReturnsNull_WhenInactive()
        {
            var mockContext = GetMockContext();

            var dresses = new List<Dress>
            {
                new Dress { Id = 5, IsActive = false }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);

            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetDressById(5);

            Assert.Null(result);
        }

        #endregion

        #region GetSizesByModelId

        [Fact]
        public async Task GetSizesByModelId_ReturnsDistinctActiveSizes()
        {
            var mockContext = GetMockContext();

            var dresses = new List<Dress>
            {
                new Dress { ModelId = 10, Size = "S", IsActive = true },
                new Dress { ModelId = 10, Size = "S", IsActive = true },
                new Dress { ModelId = 10, Size = "M", IsActive = false },
                new Dress { ModelId = 10, Size = "L", IsActive = true },
                new Dress { ModelId = 99, Size = "XL", IsActive = true }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);

            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetSizesByModelId(10);

            Assert.Equal(2, result.Count);
            Assert.Contains("S", result);
            Assert.Contains("L", result);
            Assert.DoesNotContain("M", result);
        }

        [Fact]
        public async Task GetSizesByModelId_ReturnsEmpty_WhenNoActiveDresses()
        {
            var mockContext = GetMockContext();

            var dresses = new List<Dress>
            {
                new Dress { ModelId = 10, Size = "S", IsActive = false }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);

            var repository = new DressRepository(mockContext.Object);

            var result = await repository.GetSizesByModelId(10);

            Assert.Empty(result);
        }

        #endregion

        #region GetCountByModelIdAndSizeForDate

        [Fact]
        public async Task GetCountByModelIdAndSizeForDate_ReturnsOnlyAvailableDresses()
        {
            var mockContext = GetMockContext();

            var targetDate = new DateOnly(2025, 5, 10);

            var dresses = new List<Dress>
            {
                new Dress
                {
                    Id = 1,
                    ModelId = 1,
                    Size = "M",
                    IsActive = true,
                    OrderItems = new List<OrderItem>()
                },
                new Dress
                {
                    Id = 2,
                    ModelId = 1,
                    Size = "M",
                    IsActive = true,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Order = new Order
                            {
                                EventDate = targetDate.AddDays(3) 
                            }
                        }
                    }
                },
                new Dress
                {
                    Id = 3,
                    ModelId = 1,
                    Size = "M",
                    IsActive = false,
                    OrderItems = new List<OrderItem>()
                }
            };

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(dresses);

            var repository = new DressRepository(mockContext.Object);

            var count = await repository.GetCountByModelIdAndSizeForDate(1, "M", targetDate);

            Assert.Equal(1, count); // רק הראשונה זמינה
        }

        #endregion

        #region Add / Update / Delete

        [Fact]
        public async Task AddDress_CallsSaveChangesOnce()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(new List<Dress>());

            var repository = new DressRepository(mockContext.Object);

            var dress = new Dress { Id = 10, Size = "XL", IsActive = true };

            var result = await repository.AddDress(dress);

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task UpdateDress_CallsUpdateAndSaveChanges()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(new List<Dress>());

            var repository = new DressRepository(mockContext.Object);

            var dress = new Dress { Id = 1, Size = "S" };

            await repository.UpdateDress(dress);

            mockContext.Verify(x => x.Dresses.Update(dress), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDress_CallsUpdateAndSaveChanges()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Dresses).ReturnsDbSet(new List<Dress>());

            var repository = new DressRepository(mockContext.Object);

            var dress = new Dress { Id = 1, IsActive = false };

            await repository.DeleteDress(dress);

            mockContext.Verify(x => x.Dresses.Update(dress), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
