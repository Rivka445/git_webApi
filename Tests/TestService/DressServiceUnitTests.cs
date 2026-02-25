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

namespace Services.Tests
{
    public class DressServiceTests
    {
        private readonly Mock<IDressRepository> _dressRepoMock;
        private readonly IMapper _mapper;
        private readonly DressService _dressService;

        public DressServiceTests()
        {
            _dressRepoMock = new Mock<IDressRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dress, DressDTO>()
                   .ForMember(d => d.ModelImgUrl, opt => opt.MapFrom(s => s.Model.ImgUrl));
                cfg.CreateMap<NewDressDTO, Dress>();
            });

            _mapper = config.CreateMapper();
            _dressService = new DressService(_dressRepoMock.Object, _mapper);
        }

        #region Check Methods

        [Fact]
        public void CheckPrice_Positive_ReturnsTrue()
        {
            Assert.True(_dressService.checkPrice(100));
        }

        [Fact]
        public void CheckPrice_ZeroOrNegative_ReturnsFalse()
        {
            Assert.False(_dressService.checkPrice(0));
            Assert.False(_dressService.checkPrice(-10));
        }

        [Fact]
        public void CheckDate_Future_ReturnsTrue()
        {
            var futureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            Assert.True(_dressService.checkDate(futureDate));
        }

        [Fact]
        public void CheckDate_PastOrToday_ReturnsFalse()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var past = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
            Assert.False(_dressService.checkDate(today));
            Assert.False(_dressService.checkDate(past));
        }

        #endregion

        #region GetDressById

        [Fact]
        public async Task GetDressById_Existing_ReturnsDTO()
        {
            var dress = new Dress
            {
                Id = 1,
                Size = "M",
                Price = 120,
                Model = new Model { ImgUrl = "img.jpg" }
            };

            _dressRepoMock.Setup(r => r.GetDressById(1)).ReturnsAsync(dress);

            var result = await _dressService.GetDressById(1);

            Assert.NotNull(result);
            Assert.Equal("M", result.Size);
            Assert.Equal("img.jpg", result.ModelImgUrl);
        }

        [Fact]
        public async Task GetDressById_NotFound_ReturnsNull()
        {
            _dressRepoMock.Setup(r => r.GetDressById(It.IsAny<int>())).ReturnsAsync((Dress)null);

            var result = await _dressService.GetDressById(99);

            Assert.Null(result);
        }

        #endregion

        #region GetSizesByModelId

        [Fact]
        public async Task GetSizesByModelId_ReturnsList()
        {
            var sizes = new List<string> { "S", "M", "L" };
            _dressRepoMock.Setup(r => r.GetSizesByModelId(1)).ReturnsAsync(sizes);

            var result = await _dressService.GetSizesByModelId(1);

            Assert.Equal(3, result.Count);
            Assert.Contains("M", result);
        }

        #endregion

        #region AddDress

        [Fact]
        public async Task AddDress_MapsAndReturnsDTO()
        {
            var newDress = new NewDressDTO(1, "S", 100, "");      
            _dressRepoMock.Setup(r => r.AddDress(It.IsAny<Dress>()))
                          .ReturnsAsync((Dress d) => { d.Id = 5; return d; });

            var result = await _dressService.AddDress(newDress);

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("S", result.Size);
        }

        #endregion

        #region UpdateDress

        [Fact]
        public async Task UpdateDress_CallsRepository()
        {
            var updateDto = new NewDressDTO(1, "L",  150,"" );

            await _dressService.UpdateDress(1, updateDto);

            _dressRepoMock.Verify(r => r.UpdateDress(It.Is<Dress>(d => d.Size == "L")), Times.Once);
        }

        #endregion

        #region DeleteDress

        [Fact]
        public async Task DeleteDress_CallsRepository()
        {
            await _dressService.DeleteDress(1);

            _dressRepoMock.Verify(r => r.DeleteDress(1), Times.Once);
        }

        #endregion
    }
}