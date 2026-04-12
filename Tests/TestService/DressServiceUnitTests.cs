using AutoMapper;
using DTOs;
using Entities;
using Moq;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class DressServiceUnitTests
    {
        private readonly IMapper _mapper;

        public DressServiceUnitTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dress, DressDTO>().ReverseMap();
                cfg.CreateMap<Dress, DressResponseDTO>().ReverseMap();
                cfg.CreateMap<NewDressDTO, Dress>().ReverseMap();
            });

            _mapper = config.CreateMapper();
        }

        #region IsExistsDressById

        [Fact]
        public async Task IsExistsDressById_ReturnsTrue_WhenExists()
        {
            var mockRepo = new Mock<IDressRepository>();
            mockRepo.Setup(r => r.IsExistsDressById(1)).ReturnsAsync(true);

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.IsExistsDressById(1);

            Assert.True(result);
        }

        [Fact]
        public async Task IsExistsDressById_ReturnsFalse_WhenNotExists()
        {
            var mockRepo = new Mock<IDressRepository>();
            mockRepo.Setup(r => r.IsExistsDressById(2)).ReturnsAsync(false);

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.IsExistsDressById(2);

            Assert.False(result);
        }

        #endregion

        #region CheckPrice

        [Fact]
        public void CheckPrice_ReturnsTrue_WhenPositive()
        {
            var service = new DressService(new Mock<IDressRepository>().Object, _mapper);

            Assert.True(service.CheckPrice(100));
        }

        [Fact]
        public void CheckPrice_ReturnsFalse_WhenZeroOrNegative()
        {
            var service = new DressService(new Mock<IDressRepository>().Object, _mapper);

            Assert.False(service.CheckPrice(0));
            Assert.False(service.CheckPrice(-10));
        }

        #endregion

        #region CheckDate

        [Fact]
        public void CheckDate_ReturnsTrue_WhenFuture()
        {
            var service = new DressService(new Mock<IDressRepository>().Object, _mapper);

            var future = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

            Assert.True(service.CheckDate(future));
        }

        [Fact]
        public void CheckDate_ReturnsFalse_WhenTodayOrPast()
        {
            var service = new DressService(new Mock<IDressRepository>().Object, _mapper);

            var today = DateOnly.FromDateTime(DateTime.Now);
            var past = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

            Assert.False(service.CheckDate(today));
            Assert.False(service.CheckDate(past));
        }

        #endregion

        #region IsDressAvailable

        [Fact]
        public async Task IsDressAvailable_ReturnsTrue()
        {
            var mockRepo = new Mock<IDressRepository>();
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

            mockRepo.Setup(r => r.IsDressAvailable(1, date)).ReturnsAsync(true);

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.IsDressAvailable(1, date);

            Assert.True(result);
        }

        #endregion

        #region GetPriceById

        [Fact]
        public async Task GetPriceById_ReturnsValue()
        {
            var mockRepo = new Mock<IDressRepository>();
            mockRepo.Setup(r => r.GetPriceById(1)).ReturnsAsync(250);

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.GetPriceById(1);

            Assert.Equal(250, result);
        }

        #endregion

        #region GetDressById

        [Fact]
        public async Task GetDressById_ReturnsDTO_WhenExists()
        {
            var mockRepo = new Mock<IDressRepository>();
            mockRepo.Setup(r => r.GetDressById(1))
                    .ReturnsAsync(new Dress { Id = 1 });

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.GetDressById(1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDressById_ReturnsNull_WhenNotExists()
        {
            var mockRepo = new Mock<IDressRepository>();
            mockRepo.Setup(r => r.GetDressById(1))
                    .ReturnsAsync((Dress)null);

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.GetDressById(1);

            Assert.Null(result);
        }

        #endregion

        #region GetDressesByModelId

        [Fact]
        public async Task GetDressesByModelId_ReturnsList()
        {
            var mockRepo = new Mock<IDressRepository>();

            mockRepo.Setup(r => r.GetDressesByModelId(1))
                    .ReturnsAsync(new List<Dress>
                    {
                        new Dress { Id = 1 },
                        new Dress { Id = 2 }
                    });

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.GetDressesByModelId(1);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region GetDressByModelIdAndSize

        [Fact]
        public async Task GetDressByModelIdAndSize_ReturnsDTO_WhenExists()
        {
            var mockRepo = new Mock<IDressRepository>();

            mockRepo.Setup(r => r.GetDressByModelIdAndSize(1, "M"))
                    .ReturnsAsync(new Dress { Id = 1 });

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.GetDressByModelIdAndSize(1, "M");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetDressByModelIdAndSize_ReturnsNull_WhenNotExists()
        {
            var mockRepo = new Mock<IDressRepository>();

            mockRepo.Setup(r => r.GetDressByModelIdAndSize(1, "M"))
                    .ReturnsAsync((Dress)null);

            var service = new DressService(mockRepo.Object, _mapper);

            var result = await service.GetDressByModelIdAndSize(1, "M");

            Assert.Null(result);
        }

        #endregion

        #region AddDress

        [Fact]
        public async Task AddDress_MapsAndReturnsDTO()
        {
            var mockRepo = new Mock<IDressRepository>();

            mockRepo.Setup(r => r.AddDress(It.IsAny<Dress>()))
                    .ReturnsAsync((Dress d) =>
                    {
                        d.Id = 1;
                        return d;
                    });

            var service = new DressService(mockRepo.Object, _mapper);

            var dto = new NewDressDTO(1, "M", 100, "note");

            var result = await service.AddDress(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddDress_SendsCorrectDataToRepository()
        {
            var mockRepo = new Mock<IDressRepository>();

            mockRepo.Setup(r => r.AddDress(It.IsAny<Dress>()))
                    .ReturnsAsync((Dress d) => d);

            var service = new DressService(mockRepo.Object, _mapper);

            var dto = new NewDressDTO(5, "L", 300, "test");

            await service.AddDress(dto);

            mockRepo.Verify(r => r.AddDress(It.Is<Dress>(d =>
                d.ModelId == 5 &&
                d.Size == "L" &&
                d.Price == 300 &&
                d.Note == "test" &&
                d.IsActive == true
            )), Times.Once);
        }

        #endregion

        #region UpdateDress

        [Fact]
        public async Task UpdateDress_CallsRepositoryWithCorrectId()
        {
            var mockRepo = new Mock<IDressRepository>();

            var service = new DressService(mockRepo.Object, _mapper);

            var dto = new NewDressDTO(1, "S", 150, "update");

            await service.UpdateDress(10, dto);

            mockRepo.Verify(r => r.UpdateDress(It.Is<Dress>(d =>
                d.Id == 10 &&
                d.IsActive == true
            )), Times.Once);
        }

        #endregion

        #region DeleteDress

        [Fact]
        public async Task DeleteDress_CallsRepository()
        {
            var mockRepo = new Mock<IDressRepository>();

            var service = new DressService(mockRepo.Object, _mapper);

            await service.DeleteDress(1);

            mockRepo.Verify(r => r.DeleteDress(1), Times.Once);
        }

        #endregion
    }
}