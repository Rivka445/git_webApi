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
//    public class UserServiceTests
//    {
//        private readonly Mock<IUserRepository> _userRepoMock;
//        private readonly Mock<IUserPasswordService> _passwordServiceMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly UserService _userService;

//        public UserServiceTests()
//        {
//            _userRepoMock = new Mock<IUserRepository>();
//            _passwordServiceMock = new Mock<IUserPasswordService>();
//            _mapperMock = new Mock<IMapper>();

//            _userService = new UserService(
//                _userRepoMock.Object,
//                _mapperMock.Object,
//                _passwordServiceMock.Object
//            );
//        }

//        #region Get Users

//        [Fact]
//        public async Task GetUsers_ReturnsListOfUserDTOs()
//        {
//            var users = new List<User> { new User { Id = 1, FirstName = "Test" } };
//            var usersDto = new List<UserDTO> { new UserDTO(1, "Test", "User", "t@t.com", "050", "123456") };

//            _userRepoMock.Setup(r => r.GetUsers()).ReturnsAsync(users);
//            _mapperMock.Setup(m => m.Map<List<User>, List<UserDTO>>(users)).Returns(usersDto);

//            var result = await _userService.GetUsers();

//            Assert.Single(result);
//            Assert.Equal("Test", result[0].FirstName);
//        }

//        [Fact]
//        public async Task GetUserById_ExistingId_ReturnsUserDTO()
//        {
//            int id = 1;
//            var user = new User { Id = id, FirstName = "Test" };
//            var userDto = new UserDTO(id, "Test", "User", "t@t.com", "050", "123456");

//            _userRepoMock.Setup(r => r.GetUserById(id)).ReturnsAsync(user);
//            _mapperMock.Setup(m => m.Map<User, UserDTO>(user)).Returns(userDto);

//            var result = await _userService.GetUserById(id);

//            Assert.NotNull(result);
//            Assert.Equal(id, result.Id);
//        }

//        [Fact]
//        public async Task GetUserById_NonExistingId_ThrowsKeyNotFoundException()
//        {
//            _userRepoMock.Setup(r => r.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetUserById(999));
//        }

//        [Fact]
//        public async Task GetUserById_InvalidId_ThrowsKeyNotFoundException() // שים לב לשם ולסוג
//        {
//            // Arrange
//            _userRepoMock.Setup(r => r.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetUserById(0));
//        }

//        #endregion

//        #region Registration & Login

//        [Fact]
//        public async Task AddUser_ValidUser_ReturnsUserDTO()
//        {
//            var registerDto = new UserRegisterDTO("First", "Last", "e@e.com", "050", "SecurePass123");
//            var userEntity = new User { FirstName = "First", Password = "SecurePass123" };
//            var savedUser = new User { Id = 1, FirstName = "First" };
//            var expectedDto = new UserDTO(1, "First", "Last", "e@e.com", "050", "SecurePass123");

//            _passwordServiceMock.Setup(p => p.CheckPassword(registerDto.Password)).Returns(4);

//            _mapperMock.Setup(m => m.Map<UserRegisterDTO, User>(registerDto)).Returns(userEntity);
//            _userRepoMock.Setup(r => r.AddUser(userEntity)).ReturnsAsync(savedUser);
//            _mapperMock.Setup(m => m.Map<User, UserDTO>(savedUser)).Returns(expectedDto);

//            var result = await _userService.AddUser(registerDto);

//            Assert.Equal(1, result.Id);
//            _userRepoMock.Verify(r => r.AddUser(It.IsAny<User>()), Times.Once);
//        }

//        [Fact]
//        public async Task AddUser_WeakPassword_ThrowsArgumentException()
//        {
//            var weakUser = new UserRegisterDTO("F", "L", "e@e.com", "050", "123");
//            _passwordServiceMock.Setup(p => p.CheckPassword("123")).Returns(1);

//            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _userService.AddUser(weakUser));
//            Assert.Equal("Password is too weak.", ex.Message);
//        }

//        [Fact]
//        public async Task AddUser_NullUser_ThrowsArgumentNullException()
//        {
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.AddUser(null));
//        }

//        [Fact]
//        public async Task LogIn_ValidCredentials_ReturnsUserDTO()
//        {
//            var loginDto = new UserLoginDTO("First", "Last", "Pass");
//            var loginUser = new User { FirstName = "First", LastName = "Last", Password = "Pass" };
//            var dbUser = new User { Id = 1, FirstName = "First" };
//            var expectedDto = new UserDTO(1, "First", "Last", "e@e.com", "050", "Pass");

//            _mapperMock.Setup(m => m.Map<UserLoginDTO, User>(loginDto)).Returns(loginUser);
//            _userRepoMock.Setup(r => r.LogIn(loginUser)).ReturnsAsync(dbUser);
//            _mapperMock.Setup(m => m.Map<User, UserDTO>(dbUser)).Returns(expectedDto);

//            var result = await _userService.LogIn(loginDto);

//            Assert.NotNull(result);
//            Assert.Equal(1, result.Id);
//        }

//        [Fact]
//        public async Task LogIn_InvalidCredentials_ThrowsUnauthorizedAccessException()
//        {
//            var loginDto = new UserLoginDTO("Wrong", "User", "Pass");
//            _mapperMock.Setup(m => m.Map<UserLoginDTO, User>(loginDto)).Returns(new User());
//            _userRepoMock.Setup(r => r.LogIn(It.IsAny<User>())).ReturnsAsync((User)null);

//            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LogIn(loginDto));
//        }

//        [Fact]
//        public async Task LogIn_NullUser_ThrowsArgumentNullException()
//        {
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.LogIn(null));
//        }

//        #endregion

//        #region Update User

//        [Fact]
//        public async Task UpdateUser_ValidUpdate_CallsRepository()
//        {
//            int id = 1;
//            var updateDto = new UserDTO(id, "Updated", "User", "e@e.com", "050", "StrongPass!");
//            var userEntity = new User { Id = id, FirstName = "Updated" };

//            _passwordServiceMock.Setup(p => p.CheckPassword(updateDto.Password)).Returns(5);
//            _mapperMock.Setup(m => m.Map<UserDTO, User>(updateDto)).Returns(userEntity);

//            await _userService.UpdateUser(id, updateDto);

//            _userRepoMock.Verify(r => r.UpdateUser(userEntity), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateUser_WeakPassword_ThrowsArgumentException()
//        {
//            int id = 1;
//            var updateDto = new UserDTO(id, "Updated", "User", "e@e.com", "050", "123");
//            _passwordServiceMock.Setup(p => p.CheckPassword("123")).Returns(1);

//            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUser(id, updateDto));
//        }

//        [Fact]
//        public async Task UpdateUser_NullDto_ThrowsArgumentNullException()
//        {
//            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateUser(1, null));
//        }

//        [Fact]
//        public async Task UpdateUser_InvalidId_ThrowsArgumentException()
//        {
//            var updateDto = new UserDTO(0, "Test", "User", "e@e.com", "050", "Pass");
//            await Assert.ThrowsAsync<ArgumentException>(() => _userService.UpdateUser(0, updateDto));
//        }

//        #endregion
//    }
//}
