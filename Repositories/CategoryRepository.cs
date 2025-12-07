using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WebApiShopContext _webApiShopContext;
        public CategoryRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }
        public async Task<List<Category>> GetCategories()
        {
            return await _webApiShopContext.Categories.ToListAsync();
        }
    }

}
