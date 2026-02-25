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
	public class OrderRepositoryIntegrationTests : IAsyncLifetime
	{
		private readonly EventDressRentalContext _dbContext;
		private readonly OrderRepository _orderRepository;

		public OrderRepositoryIntegrationTests(DatabaseFixture fixture)
		{
			_dbContext = fixture.Context;
			_orderRepository = new OrderRepository(_dbContext);
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
		public async Task AddOrder_ReturnsOrderWithUserAndStatus()
		{
			var user = new User
			{
				FirstName = "Test",
				LastName = "User",
				Email = "testuser@example.com",
				Phone = "0500000000",
				Password = "password123",
				Role = "User"
			};
			var status = new Status { Name = "New" };
			var category = new Category { Name = "Evening" };
			var model = new Model
			{
				Name = "Model A",
				Description = "Desc",
				ImgUrl = "img.png",
				BasePrice = 200,
				Color = "Black",
				IsActive = true,
				Categories = new List<Category> { category }
			};
			var dress = new Dress
			{
				Model = model,
				Size = "M",
				Price = 220,
				Note = "Note",
				IsActive = true
			};

			await _dbContext.Users.AddAsync(user);
			await _dbContext.Statuses.AddAsync(status);
			await _dbContext.Dresses.AddAsync(dress);
			await _dbContext.SaveChangesAsync();

			var order = new Order
			{
				UserId = user.Id,
				StatusId = status.Id,
				OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
				EventDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
				FinalPrice = 220,
				Note = "Order note",
				OrderItems = new List<OrderItem>
				{
					new OrderItem { DressId = dress.Id }
				}
			};

			var result = await _orderRepository.AddOrder(order);

			Assert.NotNull(result);
			Assert.Equal(user.Id, result!.UserId);
			Assert.Equal(status.Id, result.StatusId);
			Assert.NotNull(result.User);
			Assert.NotNull(result.Status);
		}

		[Fact]
		public async Task GetOrdersByDate_ReturnsOnlyMatchingStatusAndDate()
		{
			var user = new User
			{
				FirstName = "Client",
				LastName = "One",
				Email = "client1@example.com",
				Phone = "0500000001",
				Password = "password123",
				Role = "User"
			};
			await _dbContext.Users.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			await _dbContext.Database.ExecuteSqlRawAsync(
				"SET IDENTITY_INSERT Statuses ON;" +
				"INSERT INTO Statuses (id, name) VALUES (1, 'New');" +
				"INSERT INTO Statuses (id, name) VALUES (2, 'Other');" +
				"SET IDENTITY_INSERT Statuses OFF;");

			var targetDate = new DateOnly(2026, 2, 1);
			var orders = new List<Order>
			{
				new Order { UserId = user.Id, StatusId = 1, OrderDate = targetDate, EventDate = targetDate.AddDays(-1), FinalPrice = 100, Note = "A" },
				new Order { UserId = user.Id, StatusId = 1, OrderDate = targetDate, EventDate = targetDate.AddDays(2), FinalPrice = 120, Note = "B" },
				new Order { UserId = user.Id, StatusId = 2, OrderDate = targetDate, EventDate = targetDate.AddDays(-1), FinalPrice = 130, Note = "C" }
			};

			await _dbContext.Orders.AddRangeAsync(orders);
			await _dbContext.SaveChangesAsync();

			var result = await _orderRepository.GetOrdersByDate(targetDate);

			Assert.Single(result);
			Assert.Equal(orders[0].Id, result[0].Id);
		}

		[Fact]
		public async Task UpdateStatusOrder_UpdatesStatusId()
		{
			var user = new User
			{
				FirstName = "Client",
				LastName = "Two",
				Email = "client2@example.com",
				Phone = "0500000002",
				Password = "password123",
				Role = "User"
			};
			var statusNew = new Status { Name = "New" };
			var statusDone = new Status { Name = "Done" };
			await _dbContext.Users.AddAsync(user);
			await _dbContext.Statuses.AddRangeAsync(statusNew, statusDone);
			await _dbContext.SaveChangesAsync();

			var order = new Order
			{
				UserId = user.Id,
				StatusId = statusNew.Id,
				OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
				EventDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
				FinalPrice = 150,
				Note = "Update status"
			};
			await _dbContext.Orders.AddAsync(order);
			await _dbContext.SaveChangesAsync();

			order.StatusId = statusDone.Id;
			await _orderRepository.UpdateStatusOrder(order);

			var updated = await _dbContext.Orders.FindAsync(order.Id);
			Assert.NotNull(updated);
			Assert.Equal(statusDone.Id, updated!.StatusId);
		}
	}
}

