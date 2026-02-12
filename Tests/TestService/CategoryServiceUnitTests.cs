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
            #region Mapper Configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDTO>().ReverseMap();
            });
            _mapper = config.CreateMapper();
            #endregion
        }

        #region GetCategories Tests

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var categories = new List<Category>
            {
                new Category { Name = "Electronics", Description = "All electronics" },
                new Category { Name = "Books", Description = "Books and magazines" }
            };
            mockRepo.Setup(r => r.GetCategories()).ReturnsAsync(categories);
            var service = new CategoryService(mockRepo.Object, _mapper);

            // Act
            var result = await service.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Name == "Electronics" && r.Description == "All electronics");
            Assert.Contains(result, r => r.Name == "Books" && r.Description == "Books and magazines");
        }

        #endregion

        #region AddCategory Tests

        [Fact]
        public async Task AddCategory_ReturnsCategoryDTO_WhenValid()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var dto = new CategoryDTO("Hats", "Fashion hats");

            mockRepo.Setup(r => r.AddCategory(It.IsAny<Category>()))
                    .ReturnsAsync((Category c) =>
                    {
                        c.Id = 1; 
                        return c;
                    });

            var service = new CategoryService(mockRepo.Object, _mapper);

            // Act
            var result = await service.AddCategory(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Hats", result.Name);
            Assert.Equal("Fashion hats", result.Description);
        }

        [Fact]
        public async Task AddCategory_CanHandleEmptyOrNullName()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var service = new CategoryService(mockRepo.Object, _mapper);

            var dtoEmpty = new CategoryDTO("", "Empty name");
            var dtoNull = new CategoryDTO(null, "Null name");

            mockRepo.Setup(r => r.AddCategory(It.IsAny<Category>()))
                    .ReturnsAsync((Category c) =>
                    {
                        c.Id = 1;
                        return c;
                    });

            // Act
            var resultEmpty = await service.AddCategory(dtoEmpty);
            var resultNull = await service.AddCategory(dtoNull);

            // Assert
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
