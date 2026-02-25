using Entities;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Tests;
using Microsoft.EntityFrameworkCore;

namespace TestProject
{
	[Collection("Database Collection")]
	public class ModelRepositoryIntegrationTests : IAsyncLifetime
	{
		private readonly EventDressRentalContext _dbContext;
		private readonly ModelRepository _modelRepository;

		public ModelRepositoryIntegrationTests(DatabaseFixture fixture)
		{
			_dbContext = fixture.Context;
			_modelRepository = new ModelRepository(_dbContext);
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
		public async Task AddModel_SavesWithCategories()
		{
			var category = new Category { Name = "Evening" };
			await _dbContext.Categories.AddAsync(category);
			await _dbContext.SaveChangesAsync();

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

			var result = await _modelRepository.AddModel(model);

			Assert.NotNull(result);
			var saved = await _dbContext.Models.FindAsync(result.Id);
			Assert.NotNull(saved);
			Assert.Equal("Model A", saved!.Name);
		}

		[Fact]
		public async Task GetModelById_ReturnsModelWithCategories()
		{
			var category = new Category { Name = "Brides" };
			var model = new Model
			{
				Name = "Model B",
				Description = "Desc",
				ImgUrl = "img.png",
				BasePrice = 300,
				Color = "White",
				IsActive = true,
				Categories = new List<Category> { category }
			};
			await _dbContext.Models.AddAsync(model);
			await _dbContext.SaveChangesAsync();

			var result = await _modelRepository.GetModelById(model.Id);

			Assert.NotNull(result);
			Assert.Single(result!.Categories);
			Assert.Equal("Brides", result.Categories.First().Name);
		}

		[Fact]
		public async Task GetModels_ReturnsPagedAndTotal()
		{
			var category = new Category { Name = "Evening" };
			var models = new List<Model>
			{
				new Model { Name = "Model C", Description = "Desc", ImgUrl = "img.png", BasePrice = 100, Color = "Red", IsActive = true, Categories = new List<Category> { category } },
				new Model { Name = "Model D", Description = "Desc", ImgUrl = "img.png", BasePrice = 200, Color = "Blue", IsActive = true, Categories = new List<Category> { category } }
			};

			await _dbContext.Models.AddRangeAsync(models);
			await _dbContext.SaveChangesAsync();

			var (items, totalCount) = await _modelRepository.GetModels(null, null, null, Array.Empty<int>(), Array.Empty<string>());

			Assert.Equal(2, totalCount);
			Assert.Equal(2, items.Count);
		}
	}
}
