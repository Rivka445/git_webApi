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
    public class UserServiceUnitTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IUserPasswordService> _passwordServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceUnitTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _passwordServiceMock = new Mock<IUserPasswordService>();
            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(
                _userRepoMock.Object,
                _mapperMock.Object,
                _passwordServiceMock.Object
            );
        }

        #region User Existence & General

        [Fact]
        public async Task IsExistsUserById_ExistingId_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _userRepoMock.Setup(r => r.IsExistsUserById(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.IsExistsUserById(userId);

            // Assert
            Assert.True(result);
            _userRepoMock.Verify(r => r.IsExistsUserById(userId), Times.Once);
        }

        [Fact]
        public void CheckUser_Always_ReturnsTrue()
        {
            // Act
            var result = _userService.CheckUser(1);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Get Users

        [Fact]
        public async Task GetUsers_ReturnsListOfUserDTOs()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, FirstName = "Test" } };
            var usersDto = new List<UserDTO> { new UserDTO(1, "Test", "User", "t@t.com", "050", "123456", "") };

            _userRepoMock.Setup(r => r.GetUsers()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<User>, List<UserDTO>>(users)).Returns(usersDto);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test", result[0].FirstName);
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsUserDTO()
        {
            // Arrange
            int id = 1;
            var user = new User { Id = id, FirstName = "Test" };
            var userDto = new UserDTO(id, "Test", "User", "t@t.com", "050", "123456", "");

            _userRepoMock.Setup(r => r.GetUserById(id)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<User, UserDTO>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNull()
        {
            // Arrange
            int id = 999;
            _userRepoMock.Setup(r => r.GetUserById(id)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserById(id);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Registration & Login

        [Fact]
        public async Task AddUser_ValidUser_ReturnsUserDTO()
        {
            // Arrange
            var registerDto = new UserRegisterDTO("First", "Last", "e@e.com", "050", "Pass123", "");
            var userEntity = new User { FirstName = "First", Password = "Pass123" };
            var savedUser = new User { Id = 1, FirstName = "First" };
            var expectedDto = new UserDTO(1, "First", "Last", "e@e.com", "050", "Pass123", "");

            _mapperMock.Setup(m => m.Map<UserRegisterDTO, User>(registerDto)).Returns(userEntity);
            _userRepoMock.Setup(r => r.AddUser(userEntity)).ReturnsAsync(savedUser);
            _mapperMock.Setup(m => m.Map<User, UserDTO>(savedUser)).Returns(expectedDto);

            // Act
            var result = await _userService.AddUser(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _userRepoMock.Verify(r => r.AddUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LogIn_ValidCredentials_ReturnsUserDTO()
        {
            // Arrange
            var loginDto = new UserLoginDTO("First", "Last", "Pass");
            var loginUser = new User { FirstName = "First", LastName = "Last", Password = "Pass" };
            var dbUser = new User { Id = 1, FirstName = "First" };
            var expectedDto = new UserDTO(1, "First", "Last", "e@e.com", "050", "Pass", "");

            _mapperMock.Setup(m => m.Map<UserLoginDTO, User>(loginDto)).Returns(loginUser);
            _userRepoMock.Setup(r => r.LogIn(loginUser)).ReturnsAsync(dbUser);
            _mapperMock.Setup(m => m.Map<User, UserDTO>(dbUser)).Returns(expectedDto);

            // Act
            var result = await _userService.LogIn(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task LogIn_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var loginDto = new UserLoginDTO("Wrong", "User", "Pass");
            _mapperMock.Setup(m => m.Map<UserLoginDTO, User>(loginDto)).Returns(new User());
            _userRepoMock.Setup(r => r.LogIn(It.IsAny<User>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.LogIn(loginDto);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Update User

        [Fact]
        public async Task UpdateUser_ValidUpdate_CallsRepository()
        {
            // Arrange
            int id = 1;
            var updateDto = new UserRegisterDTO("Updated", "User", "e@e.com", "050", "Pass!", "");
            var userEntity = new User { Id = id, FirstName = "Updated" };

            _mapperMock.Setup(m => m.Map<UserRegisterDTO, User>(updateDto)).Returns(userEntity);

            // Act
            await _userService.UpdateUser(id, updateDto);

            // Assert
            _userRepoMock.Verify(r => r.UpdateUser(userEntity), Times.Once);
        }

        #endregion
    }
}