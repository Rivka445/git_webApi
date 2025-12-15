using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly WebApiShopContext _webApiShopContext;
        public ProductRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }
        public async Task<List<Product>> GetProducts(string? description, int? minPrice, int? maxPrice,
            int?[] categoriesId, int? position , int? skip)
        {
            return await _webApiShopContext.Products.Include(p => p.Category).ToListAsync();
        }
        public async Task<Product> GetById(int id)
        {
            return await _webApiShopContext.Products.Include(p => p.Category).FirstOrDefaultAsync(o=>o.Id==id);
        }
    }
}
