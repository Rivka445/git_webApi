using Moq;
using Xunit;
using Services;
using Repositories;
using Entities;
using DTOs;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Services.Tests
{
    public class ModelServiceAdditionalTests
    {
        private readonly Mock<IModelRepository> _modelRepoMock;
        private readonly Mock<IDressService> _dressServiceMock;
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ModelService _modelService;

        public ModelServiceAdditionalTests()
        {
            _modelRepoMock = new Mock<IModelRepository>();
            _dressServiceMock = new Mock<IDressService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _mapperMock = new Mock<IMapper>();

            _modelService = new ModelService(
                _modelRepoMock.Object,
                _mapperMock.Object,
                _dressServiceMock.Object,
                _categoryServiceMock.Object
            );
        }

        #region IsExistsModelById Tests

        [Fact]
        public async Task IsExistsModelById_ModelExists_ReturnsTrue()
        {
            int id = 1;
            _modelRepoMock.Setup(r => r.IsExistsModelById(id)).ReturnsAsync(true);

            var result = await _modelService.IsExistsModelById(id);

            Assert.True(result);
        }

        [Fact]
        public async Task IsExistsModelById_ModelDoesNotExist_ReturnsFalse()
        {
            int id = 2;
            _modelRepoMock.Setup(r => r.IsExistsModelById(id)).ReturnsAsync(false);

            var result = await _modelService.IsExistsModelById(id);

            Assert.False(result);
        }

        #endregion

        #region CheckCategories Tests

        [Fact]
        public async Task CheckCategories_AllExist_ReturnsTrue()
        {
            var categories = new List<int> { 1, 2, 3 };
            foreach (var c in categories)
                _categoryServiceMock.Setup(s => s.IsExistsCategoryById(c)).ReturnsAsync(true);

            var result = await _modelService.checkCategories(categories);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckCategories_OneDoesNotExist_ReturnsFalse()
        {
            var categories = new List<int> { 1, 2, 3 };
            _categoryServiceMock.Setup(s => s.IsExistsCategoryById(1)).ReturnsAsync(true);
            _categoryServiceMock.Setup(s => s.IsExistsCategoryById(2)).ReturnsAsync(false);
            _categoryServiceMock.Setup(s => s.IsExistsCategoryById(3)).ReturnsAsync(true);

            var result = await _modelService.checkCategories(categories);

            Assert.False(result);
        }

        #endregion

        #region CheckPrice Tests

        [Theory]
        [InlineData(100, true)]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(-5, false)]
        public void CheckPrice_VariousPrices_ReturnsExpected(int price, bool expected)
        {
            var result = _modelService.checkPrice(price);
            Assert.Equal(expected, result);
        }

        #endregion

        #region ValidateQueryParameters Tests

        [Theory]
        [InlineData(0, 0, null, null, true)]
        [InlineData(1, 5, 50, 100, true)]
        [InlineData(0, 5, 100, 50, false)]
        [InlineData(-1, 5, null, null, false)]
        [InlineData(1, -5, null, null, false)]
        public void ValidateQueryParameters_VariousInputs_ReturnsExpected(int position, int skip, int? minPrice, int? maxPrice, bool expected)
        {
            var result = _modelService.ValidateQueryParameters(position, skip, minPrice, maxPrice);
            Assert.Equal(expected, result);
        }

        #endregion
    }
}