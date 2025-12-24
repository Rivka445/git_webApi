using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class UserRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly WebApiShopContext _dbContext;
        private readonly UserRipository _userRepository;
        public UserRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _userRepository = new UserRipository(_dbContext);
        }
        [Fact]
        public async Task AddUser()
        {
            // Arrange
            var newUser = new User
            {
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "securepassword"
            };

            // Act
            var result = await _userRepository.AddUser(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUserById()
        {
            // Arrange
            var user = new User
            {
                Email = "existinguser@example.com",
                FirstName = "Existing",
                LastName = "User",
                Password = "securepassword"
            };

            await _userRepository.AddUser(user);

            // Act
            var result = await _userRepository.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LogIn()
        {
            // Arrange
            var user = new User
            {
                Email = "loginuser@example.com",
                FirstName = "Login",
                LastName = "User",
                Password = "securepassword!!!11"
            };

            await _userRepository.AddUser(user);
            var loginUser = new User { Email = "loginuser@example.com", Password = "securepassword!!!11" };

            // Act
            var result = await _userRepository.LogIn(loginUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LogIn_InvalidCredentials()
        {
            // Arrange
            // Attempt to log in with incorrect credentials

            var loginUser = new User { Email = "wronguser@example.com", Password = "wrongpassword!!!!@@@@" };

            // Act
            var result = await _userRepository.LogIn(loginUser);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUsers()
        {
            // Arrange
            var user1 = new User
            {
                Email = "user1@example.com",
                FirstName = "User1",
                LastName = "Test",
                Password = "password123"
            };

            var user2 = new User
            {
                Email = "user2@example.com",
                FirstName = "User2",
                LastName = "Test",
                Password = "password123"
            };

            await _userRepository.AddUser(user1);
            await _userRepository.AddUser(user2);

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUserById_NotFound()
        {
            // Arrange
            // No user with this ID exists

            // Act
            var result = await _userRepository.GetUserById(999); // Assuming 999 does not exist

            // Assert
            Assert.Null(result);
        }
    }
}
