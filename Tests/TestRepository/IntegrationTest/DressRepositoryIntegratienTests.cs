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
	public class DressRepositoryIntegrationTests : IAsyncLifetime
	{
		private readonly EventDressRentalContext _dbContext;
		private readonly DressRepository _dressRepository;

		public DressRepositoryIntegrationTests(DatabaseFixture fixture)
		{
			_dbContext = fixture.Context;
			_dressRepository = new DressRepository(_dbContext);
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
		public async Task GetDressById_ReturnsModel_WhenActive()
		{
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

			await _dbContext.Dresses.AddAsync(dress);
			await _dbContext.SaveChangesAsync();

			var result = await _dressRepository.GetDressById(dress.Id);

			Assert.NotNull(result);
			Assert.NotNull(result!.Model);
			Assert.Equal(model.Name, result.Model.Name);
		}

		[Fact]
		public async Task IsDressAvailable_ReturnsFalse_WhenOrderInRange()
		{
			var user = new User
			{
				FirstName = "Client",
				LastName = "One",
				Email = "client@example.com",
				Phone = "0500000007",
				Password = "password123",
				Role = "User"
			};
			var status = new Status { Name = "New" };
			var category = new Category { Name = "Evening" };
			var model = new Model
			{
				Name = "Model B",
				Description = "Desc",
				ImgUrl = "img.png",
				BasePrice = 200,
				Color = "Blue",
				IsActive = true,
				Categories = new List<Category> { category }
			};
			var dress = new Dress
			{
				Model = model,
				Size = "S",
				Price = 200,
				Note = "Note",
				IsActive = true
			};

			await _dbContext.Users.AddAsync(user);
			await _dbContext.Statuses.AddAsync(status);
			await _dbContext.Dresses.AddAsync(dress);
			await _dbContext.SaveChangesAsync();

			var targetDate = new DateOnly(2026, 2, 10);
			var order = new Order
			{
				UserId = user.Id,
				StatusId = status.Id,
				OrderDate = targetDate.AddDays(-1),
				EventDate = targetDate.AddDays(2),
				FinalPrice = 200,
				Note = "Booked",
				OrderItems = new List<OrderItem>
				{
					new OrderItem { DressId = dress.Id }
				}
			};

			await _dbContext.Orders.AddAsync(order);
			await _dbContext.SaveChangesAsync();

			var isAvailable = await _dressRepository.IsDressAvailable(dress.Id, targetDate);

			Assert.False(isAvailable);
		}

		[Fact]
		public async Task AddDress_ReturnsDressWithModel()
		{
			var category = new Category { Name = "Evening" };
			var model = new Model
			{
				Name = "Model C",
				Description = "Desc",
				ImgUrl = "img.png",
				BasePrice = 250,
				Color = "White",
				IsActive = true,
				Categories = new List<Category> { category }
			};
			await _dbContext.Models.AddAsync(model);
			await _dbContext.SaveChangesAsync();

			var dress = new Dress
			{
				ModelId = model.Id,
				Size = "L",
				Price = 280,
				Note = "Note",
				IsActive = true
			};

			var result = await _dressRepository.AddDress(dress);

			Assert.NotNull(result);
			Assert.NotNull(result.Model);
			Assert.Equal(model.Id, result.Model.Id);
		}
	}
}