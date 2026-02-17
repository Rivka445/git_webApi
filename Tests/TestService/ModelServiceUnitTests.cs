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
//    public class ModelServiceTests
//    {
//        private readonly Mock<IModelRepository> _modelRepoMock;
//        private readonly Mock<IDressService> _dressServiceMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly ModelService _modelService;

//        public ModelServiceTests()
//        {
//            _modelRepoMock = new Mock<IModelRepository>();
//            _dressServiceMock = new Mock<IDressService>();
//            _mapperMock = new Mock<IMapper>();

//            _modelService = new ModelService(
//                _modelRepoMock.Object,
//                _mapperMock.Object,
//                _dressServiceMock.Object
//            );
//        }

//        #region GetModelById Tests

//        [Fact]
//        public async Task GetModelById_ExistingId_ReturnsModelDTO()
//        {
//            // Arrange
//            int modelId = 1;
//            var model = new Model { Id = modelId, Name = "Model A" };
//            var modelDto = new ModelDTO(modelId, "Model A", "D", "U", 100, "C", true, new List<CategoryDTO>());

//            _modelRepoMock.Setup(r => r.GetModelById(modelId)).ReturnsAsync(model);
//            _mapperMock.Setup(m => m.Map<Model, ModelDTO>(model)).Returns(modelDto);

//            // Act
//            var result = await _modelService.GetModelById(modelId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(modelId, result.Id);
//        }

//        [Fact]
//        public async Task GetModelById_NonExistingId_ThrowsKeyNotFoundException()
//        {
//            // Arrange
//            _modelRepoMock.Setup(r => r.GetModelById(It.IsAny<int>())).ReturnsAsync((Model)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _modelService.GetModelById(999));
//        }

//        #endregion

//        #region GetModels with Filters and Pagination Tests

//        [Fact]
//        public async Task GetModels_ValidPagination_ReturnsFinalModels()
//        {
//            // Arrange
//            var models = new List<Model>
//            {
//                new Model { Id = 1, BasePrice = 100 },
//                new Model { Id = 2, BasePrice = 200 }
//            };
//            _modelRepoMock.Setup(r => r.GetModels(null, null, null, It.IsAny<int[]>(), null, 1, 2))
//                .ReturnsAsync((models, 5));
//            _mapperMock.Setup(m => m.Map<List<Model>, List<ModelDTO>>(models))
//                       .Returns(new List<ModelDTO>
//                       {
//                           new ModelDTO(1,"M1","D","U",100,"Red",true,new List<CategoryDTO>()),
//                           new ModelDTO(2,"M2","D","U",200,"Blue",true,new List<CategoryDTO>())
//                       });

//            // Act
//            var result = await _modelService.GetModelds(null, null, null, Array.Empty<int>(), null, 1, 2);

//            // Assert
//            Assert.Equal(2, result.Items.Count);
//            Assert.True(result.HasNext);
//            Assert.False(result.HasPrev);
//        }

//        [Theory]
//        [InlineData(0, 5)]
//        [InlineData(1, 0)]
//        [InlineData(1, 5, 200, 100)]
//        public async Task GetModels_InvalidParameters_ThrowsArgumentException(int position, int skip, int? minPrice = null, int? maxPrice = null)
//        {
//            await Assert.ThrowsAsync<ArgumentException>(() =>
//                _modelService.GetModelds(null, minPrice, maxPrice, Array.Empty<int>(), null, position, skip));
//        }

//        #endregion

//        #region AddModel Tests

//        [Fact]
//        public async Task AddModel_ValidModel_ReturnsAddedModelDTO()
//        {
//            // Arrange
//            var newModelDto = new NewModelDTO("Name", "Desc", "url", 150, "Red", new List<CategoryDTO>());
//            var modelEntity = new Model { Id = 100, BasePrice = 150 };
//            var expectedDto = new ModelDTO(100, "Name", "Desc", "url", 150, "Red", true, new List<CategoryDTO>());

//            _mapperMock.Setup(m => m.Map<NewModelDTO, Model>(newModelDto)).Returns(modelEntity);
//            _modelRepoMock.Setup(r => r.AddModel(modelEntity)).ReturnsAsync(modelEntity);
//            _mapperMock.Setup(m => m.Map<Model, ModelDTO>(modelEntity)).Returns(expectedDto);

//            // Act
//            var result = await _modelService.AddModel(newModelDto);

//            // Assert
//            Assert.Equal(100, result.Id);
//            _modelRepoMock.Verify(r => r.AddModel(It.IsAny<Model>()), Times.Once);
//        }

//        [Fact]
//        public async Task AddModel_PriceZeroOrLess_ThrowsArgumentException()
//        {
//            // Arrange
//            var invalidModel = new NewModelDTO("Name", "Desc", "url", 0, "Red", new List<CategoryDTO>());

//            // Act & Assert
//            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _modelService.AddModel(invalidModel));
//            Assert.Equal("BasePrice must be greater than 0.", ex.Message);
//        }

//        [Fact]
//        public async Task AddModel_NullModel_ThrowsArgumentNullException()
//        {
//            // Act & Assert
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _modelService.AddModel(null!));
//        }

//        #endregion

//        #region UpdateModel Tests

//        [Fact]
//        public async Task UpdateModel_ValidModel_CallsRepository()
//        {
//            // Arrange
//            int id = 1;
//            var updateDto = new ModelDTO(id, "Name", "Desc", "Url", 200, "Red", true, new List<CategoryDTO>());
//            var modelEntity = new Model { Id = id, BasePrice = 200 };

//            _modelRepoMock.Setup(r => r.GetModelById(id)).ReturnsAsync(modelEntity);
//            _mapperMock.Setup(m => m.Map<ModelDTO, Model>(updateDto)).Returns(modelEntity);

//            // Act
//            await _modelService.UpdateModel(id, updateDto);

//            // Assert
//            _modelRepoMock.Verify(r => r.UpdateModel(modelEntity), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateModel_ModelNotFound_ThrowsKeyNotFoundException()
//        {
//            // Arrange
//            int id = 1;
//            var updateDto = new ModelDTO(id, "Name", "Desc", "Url", 100, "Red", true, new List<CategoryDTO>());
//            _modelRepoMock.Setup(r => r.GetModelById(id)).ReturnsAsync((Model)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _modelService.UpdateModel(id, updateDto));
//        }

//        [Fact]
//        public async Task UpdateModel_InvalidBasePrice_ThrowsArgumentException()
//        {
//            // Arrange
//            int id = 1;
//            var updateDto = new ModelDTO(id, "Name", "Desc", "Url", 0, "Red", true, new List<CategoryDTO>());

//            // Act & Assert
//            await Assert.ThrowsAsync<ArgumentException>(() => _modelService.UpdateModel(id, updateDto));
//        }

//        [Fact]
//        public async Task UpdateModel_NullModel_ThrowsArgumentNullException()
//        {
//            // Act & Assert
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _modelService.UpdateModel(1, null!));
//        }

//        #endregion

//        #region DeleteModel Tests

//        [Fact]
//        public async Task DeleteModel_ValidModel_DeactivatesModelAndDresses()
//        {
//            // Arrange
//            int modelId = 5;
//            var dresses = new List<Dress>
//            {
//                new Dress { Id = 101, ModelId = modelId, IsActive = true },
//                new Dress { Id = 102, ModelId = modelId, IsActive = true }
//            };
//            var modelEntity = new Model { Id = modelId, Dresses = dresses, IsActive = true };
//            var deleteDto = new ModelDTO(modelId, "Name", "Desc", "Url", 100, "Red", true, new List<CategoryDTO>());

//            _modelRepoMock.Setup(r => r.GetModelById(modelId)).ReturnsAsync(modelEntity);
//            _mapperMock.Setup(m => m.Map<ModelDTO, Model>(deleteDto)).Returns(modelEntity);
//            _mapperMock.Setup(m => m.Map<Dress, DressDTO>(It.IsAny<Dress>()))
//                       .Returns((Dress src) => new DressDTO(src.Id, src.ModelId, "M", 100, "", true, ""));

//            // Act
//            await _modelService.DeleteModel(modelId, deleteDto);

//            // Assert
//            Assert.False(modelEntity.IsActive);
//            _dressServiceMock.Verify(s => s.DeleteDress(It.IsAny<int>(), It.IsAny<DressDTO>()), Times.Exactly(dresses.Count));
//            _modelRepoMock.Verify(r => r.DeleteModel(It.IsAny<Model>()), Times.Once);
//        }

//        [Fact]
//        public async Task DeleteModel_ModelNotFound_ThrowsKeyNotFoundException()
//        {
//            // Arrange
//            _modelRepoMock.Setup(r => r.GetModelById(It.IsAny<int>())).ReturnsAsync((Model)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _modelService.DeleteModel(1, new ModelDTO(1, "", "", "", 100, "", true, new List<CategoryDTO>())));
//        }

//        [Fact]
//        public async Task DeleteModel_NullModel_ThrowsArgumentNullException()
//        {
//            // Act & Assert
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _modelService.DeleteModel(1, null!));
//        }

//        #endregion

 
//    }
//}
