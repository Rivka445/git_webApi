using AutoMapper;
using DTOs;
using Entities;
using Moq;
using Repositories;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class CategoryServiceUnitTests
    {
        private readonly IMapper _mapper;

        public CategoryServiceUnitTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDTO>().ReverseMap();
                cfg.CreateMap<NewCategoryDTO, Category>().ReverseMap();
            });
            _mapper = config.CreateMapper();
        }

        #region IsExistsCategoryById Tests

        [Fact]
        public async Task IsExistsCategoryById_ReturnsTrue_WhenExists()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(r => r.IsExistsCategoryById(1)).ReturnsAsync(true);
            var service = new CategoryService(mockRepo.Object, _mapper);

            // Act
            var result = await service.IsExistsCategoryById(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExistsCategoryById_ReturnsFalse_WhenNotExists()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(r => r.IsExistsCategoryById(2)).ReturnsAsync(false);
            var service = new CategoryService(mockRepo.Object, _mapper);

            // Act
            var result = await service.IsExistsCategoryById(2);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetCategories Tests

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var categories = new List<Category>
            {
                new Category { Name = "Electronics", Description = "All electronics" },
                new Category { Name = "Books", Description = "Books and magazines" }
            };
            mockRepo.Setup(r => r.GetCategories()).ReturnsAsync(categories);
            var service = new CategoryService(mockRepo.Object, _mapper);

            var result = await service.GetCategories();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Name == "Electronics" && r.Description == "All electronics");
            Assert.Contains(result, r => r.Name == "Books" && r.Description == "Books and magazines");
        }

        #endregion

        #region GetCategoryId Tests

        [Fact]
        public async Task GetCategoryId_ReturnsCategoryDTO_WhenExists()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var category = new Category { Id = 1, Name = "Toys", Description = "All toys" };
            mockRepo.Setup(r => r.GetCategoryById(1)).ReturnsAsync(category);
            var service = new CategoryService(mockRepo.Object, _mapper);

            var result = await service.GetCategoryId(1);

            Assert.NotNull(result);
            Assert.Equal("Toys", result.Name);
            Assert.Equal("All toys", result.Description);
        }

        [Fact]
        public async Task GetCategoryId_ReturnsNull_WhenNotExists()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(r => r.GetCategoryById(99)).ReturnsAsync((Category)null);
            var service = new CategoryService(mockRepo.Object, _mapper);

            var result = await service.GetCategoryId(99);

            Assert.Null(result);
        }

        #endregion

        #region AddCategory Tests

        [Fact]
        public async Task AddCategory_ReturnsCategoryDTO_WhenValid()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var newCategory = new NewCategoryDTO ("Hats", "Fashion hats");

            mockRepo.Setup(r => r.AddCategory(It.IsAny<Category>()))
                    .ReturnsAsync((Category c) =>
                    {
                        c.Id = 1;
                        return c;
                    });

            var service = new CategoryService(mockRepo.Object, _mapper);

            var result = await service.AddCategory(newCategory);

            Assert.NotNull(result);
            Assert.Equal("Hats", result.Name);
            Assert.Equal("Fashion hats", result.Description);
        }

        [Fact]
        public async Task AddCategory_CanHandleEmptyOrNullName()
        {
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object, _mapper);

            mockRepo.Setup(r => r.AddCategory(It.IsAny<Category>()))
                    .ReturnsAsync((Category c) =>
                    {
                        c.Id = 1;
                        return c;
                    });

            var emptyNameDto = new NewCategoryDTO("", "Empty name");      
            var nullNameDto = new NewCategoryDTO ( null,   "Null name" );

            var resultEmpty = await service.AddCategory(emptyNameDto);
            var resultNull = await service.AddCategory(nullNameDto);

            Assert.NotNull(resultEmpty);
            Assert.Equal("", resultEmpty.Name);
            Assert.Equal("Empty name", resultEmpty.Description);

            Assert.NotNull(resultNull);
            Assert.Null(resultNull.Name);
            Assert.Equal("Null name", resultNull.Description);
        }

        #endregion
    }
}