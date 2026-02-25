using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Tests;
using Microsoft.EntityFrameworkCore;

namespace TestProject
{
	[Collection("Database Collection")]
	public class UserRepositoryIntegrationTests : IAsyncLifetime
	{
		private readonly EventDressRentalContext _dbContext;
		private readonly UserRepository _userRepository;

		public UserRepositoryIntegrationTests(DatabaseFixture fixture)
		{
			_dbContext = fixture.Context;
			_userRepository = new UserRepository(_dbContext);
		}

		public async Task InitializeAsync()
		{
			await ClearDatabaseAsync();
		}

		public async Task DisposeAsync()
		{
			await ClearDatabaseAsync();
		}

		private async Task ClearDatabaseAsync()
		{
			_dbContext.ChangeTracker.Clear();
			_dbContext.OrderItems.RemoveRange(_dbContext.OrderItems);
			_dbContext.Orders.RemoveRange(_dbContext.Orders);
			_dbContext.Dresses.RemoveRange(_dbContext.Dresses);
			await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [Models_Categories]");
			_dbContext.Models.RemoveRange(_dbContext.Models);
			_dbContext.Categories.RemoveRange(_dbContext.Categories);
			_dbContext.Statuses.RemoveRange(_dbContext.Statuses);
			_dbContext.Users.RemoveRange(_dbContext.Users);
			await _dbContext.SaveChangesAsync();
		}

		[Fact]
		public async Task AddUser_PersistsUser()
		{
			var newUser = new User
			{
				Email = "newuser@example.com",
				FirstName = "New",
				LastName = "User",
				Phone = "0500000003",
				Password = "securepassword",
				Role = "User"
			};

			var result = await _userRepository.AddUser(newUser);

			Assert.NotNull(result);
			var saved = await _dbContext.Users.FindAsync(result.Id);
			Assert.NotNull(saved);
			Assert.Equal("newuser@example.com", saved!.Email);
		}

		[Fact]
		public async Task LogIn_ReturnsUser_WhenCredentialsMatch()
		{
			var user = new User
			{
				Email = "loginuser@example.com",
				FirstName = "Login",
				LastName = "User",
				Phone = "0500000004",
				Password = "securepassword!!!11",
				Role = "User"
			};

			await _userRepository.AddUser(user);
			var loginUser = new User
			{
				FirstName = "Login",
				LastName = "User",
				Password = "securepassword!!!11"
			};

			var result = await _userRepository.LogIn(loginUser);

			Assert.NotNull(result);
			Assert.Equal(user.Email, result!.Email);
		}

		[Fact]
		public async Task GetUsers_ReturnsAllUsers()
		{
			var user1 = new User
			{
				Email = "user1@example.com",
				FirstName = "User1",
				LastName = "Test",
				Phone = "0500000005",
				Password = "password123",
				Role = "User"
			};
			var user2 = new User
			{
				Email = "user2@example.com",
				FirstName = "User2",
				LastName = "Test",
				Phone = "0500000006",
				Password = "password123",
				Role = "User"
			};

			await _userRepository.AddUser(user1);
			await _userRepository.AddUser(user2);

			var result = await _userRepository.GetUsers();

			Assert.Equal(2, result.Count);
		}
	}
}
