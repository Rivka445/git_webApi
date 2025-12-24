using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Entities;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Tests
{
    public class DatabaseFixture : IDisposable
    {
        public WebApiShopContext Context { get; private set; }

        public DatabaseFixture()
        { 
            // Set up the test database connection and initialize the context
            var options = new DbContextOptionsBuilder<WebApiShopContext>()
                .UseSqlServer("Data Source=DESKTOP-1VUANBN; Initial Catalog=Test;Integrated Security=True;Trust Server Certificate=True;Pooling=False")
                .Options;
            Context = new WebApiShopContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            // Clean up the test database after all tests are completed
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
