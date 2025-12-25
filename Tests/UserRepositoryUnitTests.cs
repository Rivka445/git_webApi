using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace Tests
{
    public class UserRepositoryUnitTests
    {

        [Fact]
        public async Task AddUser()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var newUser = new User
            {
                Email = "newuser@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "securepassword"
            };
            var users= new List<User>() { newUser };
            mockContext.Setup(m => m.Users).ReturnsDbSet(users);
            var userRepository = new UserRipository(mockContext.Object);

            // Act
            var result = await userRepository.AddUser(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Email, result.Email);
        }

        [Fact]
        public async Task GetUserById_HappyPath()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var userRepository = new UserRipository(mockContext.Object);
            var user = new User
            {
                Id = 2,
                Email = "existinguser@example.com",
                FirstName = "Existing",
                LastName = "User",
                Password = "securepassword"
            };

            mockContext.Setup(m => m.Users.FindAsync(user.Id)).ReturnsAsync(user);

            // Act
            var result = await userRepository.GetUserById(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LogIn()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var user = new User
            {
                Email = "loginuser@example.com",
                FirstName = "Login",
                LastName = "User",
                Password = "securepassword"
            };
            var users = new List<User>() { user };
            mockContext.Setup(m => m.Users).ReturnsDbSet(users);
            var userRepository = new UserRipository(mockContext.Object);

            var loginUser = new User { Email = "loginuser@example.com", Password = "securepassword" };

            // Act
            var result = await userRepository.LogIn(loginUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LogIn_InvalidCredentials()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var user = new User
            {
                Email = "loginuser@example.com",
                FirstName = "Login",
                LastName = "User",
                Password = "securepassword"
            };
            var users = new List<User>() { user };
            mockContext.Setup(m => m.Users).ReturnsDbSet(users);
            var userRepository = new UserRipository(mockContext.Object);

            var loginUser = new User { Email = "wrong@example.com", Password = "wrongpassword" };

            // Act
            var result = await userRepository.LogIn(loginUser);

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetUsers()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var users = new List<User>
            {
            new User { Email = "user1@example.com", FirstName = "User1", LastName = "Test", Password = "password123" },
            new User {  Email = "user2@example.com", FirstName = "User2", LastName = "Test", Password = "password123" }
            };

            mockContext.Setup(m => m.Users).ReturnsDbSet(users);
            var userRepository = new UserRipository(mockContext.Object);

            // Act
            var result = await userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetUserById_NotFound()
        {
            // Arrange
            var mockContext = new Mock<WebApiShopContext>();
            var userRepository = new UserRipository(mockContext.Object);

            // No user with this ID exists
            var users = new List<User>
            {
            new User { Email = "user1@example.com", FirstName = "User1", LastName = "Test", Password = "password123" },
            new User {  Email = "user2@example.com", FirstName = "User2", LastName = "Test", Password = "password123" }
            };

            mockContext.Setup(m => m.Users).ReturnsDbSet(users);

            // Act
            var result = await userRepository.GetUserById(999); // Assuming 999 does not exist

            // Assert
            Assert.Null(result);
        }
    }

}

