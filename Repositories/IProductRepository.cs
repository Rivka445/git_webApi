using Entities;

namespace Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProducts(string? description, int? minPrice, int? maxPrice, int?[] categoriesId, int ?position , int ?skip );
        Task<Product> GetById(int id);

    }
}