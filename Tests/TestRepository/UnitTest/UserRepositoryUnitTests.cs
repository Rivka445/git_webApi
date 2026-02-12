using Entities;
using Moq;
using Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Tests
{
    public class UserRepositoryUnitTests
    {
        private Mock<EventDressRentalContext> GetMockContext()
        {
            var options = new DbContextOptionsBuilder<EventDressRentalContext>().Options;
            return new Mock<EventDressRentalContext>(options);
        }

        #region GetUserById
        [Fact]
        public async Task GetUserById_ReturnsUser_WhenExists()
        {
            // Arrange
            var mockContext = GetMockContext();

            var user = new User { Id = 1, FirstName = "Test" };
            var users = new List<User> { user };

            mockContext.Setup(x => x.Users.FindAsync(It.IsAny<object[]>()))
                       .ReturnsAsync((object[] ids) =>
                       {
                           var id = (int)ids[0];
                           return users.FirstOrDefault(u => u.Id == id);
                       });

            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.GetUserById(1);

            // Assert
            Assert.NotNull(result); 
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenNotExists()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Users)
                .ReturnsDbSet(new List<User>());

            var repository = new UserRepository(mockContext.Object);

            var result = await repository.GetUserById(99);

            Assert.Null(result);
        }

        #endregion

        #region GetUsers

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var mockContext = GetMockContext();

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "User1" },
                new User { Id = 2, FirstName = "User2" }
            };

            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var repository = new UserRepository(mockContext.Object);

            var result = await repository.GetUsers();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.FirstName == "User1");
        }

        [Fact]
        public async Task GetUsers_ReturnsEmptyList_WhenNoUsers()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Users)
                .ReturnsDbSet(new List<User>());

            var repository = new UserRepository(mockContext.Object);

            var result = await repository.GetUsers();

            Assert.Empty(result);
        }

        #endregion

        #region LogIn

        [Fact]
        public async Task LogIn_ReturnsUser_WhenCredentialsAreCorrect()
        {
            var mockContext = GetMockContext();

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "Israel", LastName = "Israeli", Password = "123" },
                new User { Id = 2, FirstName = "Sara", LastName = "Levi", Password = "456" }
            };

            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var repository = new UserRepository(mockContext.Object);

            var loginUser = new User
            {
                FirstName = "Israel",
                LastName = "Israeli",
                Password = "123"
            };

            var result = await repository.LogIn(loginUser);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task LogIn_ReturnsNull_WhenPasswordIncorrect()
        {
            var mockContext = GetMockContext();

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "Israel", LastName = "Israeli", Password = "123" }
            };

            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var repository = new UserRepository(mockContext.Object);

            var loginUser = new User
            {
                FirstName = "Israel",
                LastName = "Israeli",
                Password = "wrong"
            };

            var result = await repository.LogIn(loginUser);

            Assert.Null(result);
        }

        [Fact]
        public async Task LogIn_ReturnsNull_WhenLastNameIncorrect()
        {
            var mockContext = GetMockContext();

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "Israel", LastName = "Israeli", Password = "123" }
            };

            mockContext.Setup(x => x.Users).ReturnsDbSet(users);

            var repository = new UserRepository(mockContext.Object);

            var loginUser = new User
            {
                FirstName = "Israel",
                LastName = "Wrong",
                Password = "123"
            };

            var result = await repository.LogIn(loginUser);

            Assert.Null(result);
        }

        [Fact]
        public async Task LogIn_ReturnsNull_WhenUserNotExists()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Users)
                .ReturnsDbSet(new List<User>());

            var repository = new UserRepository(mockContext.Object);

            var loginUser = new User
            {
                FirstName = "No",
                LastName = "User",
                Password = "123"
            };

            var result = await repository.LogIn(loginUser);

            Assert.Null(result);
        }

        #endregion

        #region AddUser

        [Fact]
        public async Task AddUser_CallsSaveChangesOnce()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Users)
                .ReturnsDbSet(new List<User>());

            var repository = new UserRepository(mockContext.Object);

            var newUser = new User
            {
                FirstName = "New",
                LastName = "User",
                Password = "789"
            };

            var result = await repository.AddUser(newUser);

            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.Equal("New", result.FirstName);
        }

        #endregion

        #region UpdateUser

        [Fact]
        public async Task UpdateUser_CallsUpdateAndSave()
        {
            var mockContext = GetMockContext();

            mockContext.Setup(x => x.Users)
                .ReturnsDbSet(new List<User>());

            var repository = new UserRepository(mockContext.Object);

            var user = new User { Id = 1, FirstName = "OldName" };

            user.FirstName = "NewName";

            await repository.UpdateUser(user);

            mockContext.Verify(x => x.Users.Update(user), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
