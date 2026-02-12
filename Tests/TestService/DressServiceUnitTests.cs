using Moq;
using Xunit;
using Services;
using Repositories;
using Entities;
using DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class DressServiceUnitTests
    {
        private readonly Mock<IDressRepository> _dressRepoMock;
        private readonly Mock<IModelService> _modelServiceMock;
        private readonly IMapper _mapper;
        private readonly DressService _dressService;

        public DressServiceUnitTests()
        {
            _dressRepoMock = new Mock<IDressRepository>();
            _modelServiceMock = new Mock<IModelService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dress, DressDTO>()
                   .ForMember(d => d.ModelImgUrl, opt => opt.MapFrom(src => src.Model.ImgUrl));
                cfg.CreateMap<NewDressDTO, Dress>();
                cfg.CreateMap<DressDTO, Dress>();
            });
            _mapper = config.CreateMapper();

            _dressService = new DressService(_dressRepoMock.Object, _mapper, _modelServiceMock.Object);
        }

        #region GetDressById Tests

        [Fact]
        public async Task GetDressById_DressExists_ReturnsDressDTO()
        {
            int dressId = 1;
            var modelDto = CreateModelDto(1);
            var dress = new Dress
            {
                Id = dressId,
                ModelId = 1,
                Size = "M",
                Price = 100,
                IsActive = true,
                Model = new Model { Id = 1, ImgUrl = "img.jpg" }
            };
            _dressRepoMock.Setup(r => r.GetDressById(dressId)).ReturnsAsync(dress);

            var result = await _dressService.GetDressById(dressId);

            Assert.NotNull(result);
            Assert.Equal(dressId, result.Id);
            Assert.Equal("M", result.Size);
            Assert.Equal("img.jpg", result.ModelImgUrl);
        }

        #endregion

        #region GetSizesByModelId Tests

        [Fact]
        public async Task GetSizesByModelId_ModelExists_ReturnsSizes()
        {
            int modelId = 1;
            _modelServiceMock.Setup(s => s.GetModelById(modelId)).ReturnsAsync(CreateModelDto(modelId));
            var sizes = new List<string> { "S", "M", "L" };
            _dressRepoMock.Setup(r => r.GetSizesByModelId(modelId)).ReturnsAsync(sizes);

            var result = await _dressService.GetSizesByModelId(modelId);

            Assert.Equal(3, result.Count);
            Assert.Contains("M", result);
        }

        #endregion

        #region GetCount Tests

        [Fact]
        public async Task GetCount_ModelNotFound_ThrowsArgumentException()
        {
            _modelServiceMock.Setup(s => s.GetModelById(It.IsAny<int>())).ReturnsAsync((ModelDTO)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _dressService.GetCountByModelIdAndSizeForDate(99, "M", DateOnly.FromDateTime(DateTime.Now.AddDays(1))));
        }

        [Fact]
        public async Task GetCount_ValidParameters_ReturnsCorrectCount()
        {
            int modelId = 1;
            string size = "L";
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(5));

            _modelServiceMock.Setup(s => s.GetModelById(modelId)).ReturnsAsync(CreateModelDto(modelId));
            _dressRepoMock.Setup(r => r.GetCountByModelIdAndSizeForDate(modelId, size, date)).ReturnsAsync(3);

            var result = await _dressService.GetCountByModelIdAndSizeForDate(modelId, size, date);

            Assert.Equal(3, result);
        }

        #endregion

        #region AddDress Tests

        [Fact]
        public async Task AddDress_ModelNotFound_ThrowsArgumentException()
        {
            var newDress = new NewDressDTO(99, "S", 200, "Note");
            _modelServiceMock.Setup(s => s.GetModelById(99)).ReturnsAsync((ModelDTO)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _dressService.AddDress(newDress));
        }

        [Fact]
        public async Task AddDress_Valid_ReturnsDressDTO()
        {
            var newDress = new NewDressDTO(1, "M", 150, "Note");
            var modelDto = CreateModelDto(1);
            _modelServiceMock.Setup(s => s.GetModelById(1)).ReturnsAsync(modelDto);
            _dressRepoMock.Setup(r => r.AddDress(It.IsAny<Dress>()))
                          .ReturnsAsync((Dress d) =>
                          {
                              d.Id = 10; 
                              return d;
                          });

            var result = await _dressService.AddDress(newDress);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("M", result.Size);
        }

        #endregion

        #region UpdateDress Tests

        [Fact]
        public async Task UpdateDress_DressIdNotFound_ThrowsKeyNotFoundException()
        {
            int id = 50;
            var updateDto = new DressDTO(id, 1, "M", 250, "", true, "url");
            _dressRepoMock.Setup(r => r.GetDressById(id)).ReturnsAsync((Dress)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _dressService.UpdateDress(id, updateDto));
        }

        [Fact]
        public async Task UpdateDress_PriceIsZero_ThrowsArgumentException()
        {
            int id = 1;
            var updateDto = new DressDTO(id, 1, "M", 0, "", true, "url"); // Price = 0
            _dressRepoMock.Setup(r => r.GetDressById(id)).ReturnsAsync(new Dress { Id = id });

            await Assert.ThrowsAsync<ArgumentException>(() => _dressService.UpdateDress(id, updateDto));
        }

        #endregion

        #region DeleteDress Tests

        [Fact]
        public async Task DeleteDress_AlreadyInactive_ThrowsInvalidOperationException()
        {
            var dressDto = new DressDTO(1, 1, "M", 200, "", false, "url");

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _dressService.DeleteDress(1, dressDto));
            Assert.Equal("Dress is already inactive.", ex.Message);
        }

        [Fact]
        public async Task DeleteDress_DressIdNotFoundInDb_ThrowsKeyNotFoundException()
        {
            int id = 100;
            var dressDto = new DressDTO(id, 1, "M", 200, "", true, "url");
            _dressRepoMock.Setup(r => r.GetDressById(id)).ReturnsAsync((Dress)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _dressService.DeleteDress(id, dressDto));
        }

        #endregion

        #region Helper Methods

        private ModelDTO CreateModelDto(int id)
        {
            return new ModelDTO(id, "TestModel", "Desc", "img.jpg", 100, "Blue", true, new List<CategoryDTO>());
        }

        #endregion
    }
}
