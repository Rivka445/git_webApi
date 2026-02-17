//using Moq;
//using Xunit;
//using Services;
//using Repositories;
//using Entities;
//using DTOs;
//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Services.Tests
//{
//    public class DressServiceUnitTests
//    {
//        private readonly Mock<IDressRepository> _dressRepoMock;
//        private readonly Mock<IModelService> _modelServiceMock;
//        private readonly IMapper _mapper;
//        private readonly DressService _dressService;

//        public DressServiceUnitTests()
//        {
//            _dressRepoMock = new Mock<IDressRepository>();
//            _modelServiceMock = new Mock<IModelService>();

//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<Dress, DressDTO>()
//                   .ForMember(d => d.ModelImgUrl,
//                       opt => opt.MapFrom(src => src.Model.ImgUrl));

//                cfg.CreateMap<NewDressDTO, Dress>();
//                cfg.CreateMap<DressDTO, Dress>();
//            });

//            _mapper = config.CreateMapper();

//            _dressService = new DressService(
//                _dressRepoMock.Object,
//                _mapper,
//                _modelServiceMock.Object);
//        }

//        #region Check Methods

//        [Fact]
//        public void CheckPrice_PositivePrice_ReturnsTrue()
//        {
//            Assert.True(_dressService.checkPrice(100));
//        }

//        [Fact]
//        public void CheckPrice_ZeroOrNegative_ReturnsFalse()
//        {
//            Assert.False(_dressService.checkPrice(0));
//            Assert.False(_dressService.checkPrice(-5));
//        }

//        [Fact]
//        public void CheckDate_FutureDate_ReturnsTrue()
//        {
//            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
//            Assert.True(_dressService.checkDate(date));
//        }

//        [Fact]
//        public void CheckDate_PastOrToday_ReturnsFalse()
//        {
//            var today = DateOnly.FromDateTime(DateTime.Now);
//            var past = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

//            Assert.False(_dressService.checkDate(today));
//            Assert.False(_dressService.checkDate(past));
//        }

//        #endregion

//        #region GetDressById

//        [Fact]
//        public async Task GetDressById_DressExists_ReturnsDTO()
//        {
//            int id = 1;

//            var dress = new Dress
//            {
//                Id = id,
//                ModelId = 1,
//                Size = "M",
//                Price = 100,
//                IsActive = true,
//                Model = new Model { Id = 1, ImgUrl = "img.jpg" }
//            };

//            _dressRepoMock.Setup(r => r.GetDressById(id))
//                          .ReturnsAsync(dress);

//            var result = await _dressService.GetDressById(id);

//            Assert.NotNull(result);
//            Assert.Equal(id, result.Id);
//            Assert.Equal("M", result.Size);
//            Assert.Equal("img.jpg", result.ModelImgUrl);
//        }

//        [Fact]
//        public async Task GetDressById_NotFound_ReturnsNull()
//        {
//            _dressRepoMock.Setup(r => r.GetDressById(It.IsAny<int>()))
//                          .ReturnsAsync((Dress)null);

//            var result = await _dressService.GetDressById(99);

//            Assert.Null(result);
//        }

//        #endregion

//        #region GetSizesByModelId

//        [Fact]
//        public async Task GetSizesByModelId_ReturnsSizesFromRepository()
//        {
//            var sizes = new List<string> { "S", "M", "L" };

//            _dressRepoMock.Setup(r => r.GetSizesByModelId(1))
//                          .ReturnsAsync(sizes);

//            var result = await _dressService.GetSizesByModelId(1);

//            Assert.Equal(3, result.Count);
//            Assert.Contains("M", result);
//        }

//        #endregion

//        #region GetCount

//        [Fact]
//        public async Task GetCount_ReturnsValueFromRepository()
//        {
//            int modelId = 1;
//            string size = "L";
//            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

//            _dressRepoMock.Setup(r =>
//                r.GetCountByModelIdAndSizeForDate(modelId, size, date))
//                .ReturnsAsync(5);

//            var result = await _dressService
//                .GetCountByModelIdAndSizeForDate(modelId, size, date);

//            Assert.Equal(5, result);
//        }

//        #endregion

//        #region AddDress

//        [Fact]
//        public async Task AddDress_Valid_ReturnsMappedDTO()
//        {
//            var newDress = new NewDressDTO(1, "M", 150, "Note");

//            _dressRepoMock.Setup(r => r.AddDress(It.IsAny<Dress>()))
//                          .ReturnsAsync((Dress d) =>
//                          {
//                              d.Id = 10;
//                              return d;
//                          });

//            var result = await _dressService.AddDress(newDress);

//            Assert.NotNull(result);
//            Assert.Equal(10, result.Id);
//            Assert.Equal("M", result.Size);
//        }

//        #endregion

//        #region UpdateDress

//        [Fact]
//        public async Task UpdateDress_CallsRepository()
//        {
//            int id = 1;
//            var dto = new DressDTO(id, 1, "M", 200, "", true, "img.jpg");

//            await _dressService.UpdateDress(id, dto);

//            _dressRepoMock.Verify(r =>
//                r.UpdateDress(It.Is<Dress>(d => d.Id == id)),
//                Times.Once);
//        }

//        #endregion

//        #region DeleteDress

//        [Fact]
//        public async Task DeleteDress_SetsIsActiveFalse_AndCallsRepository()
//        {
//            int id = 1;
//            var dto = new DressDTO(id, 1, "M", 200, "", true, "img.jpg");

//            await _dressService.DeleteDress(id, dto);

//            _dressRepoMock.Verify(r =>
//                r.DeleteDress(It.Is<Dress>(d =>
//                    d.Id == id && d.IsActive == false)),
//                Times.Once);
//        }

//        #endregion
//    }
//}
