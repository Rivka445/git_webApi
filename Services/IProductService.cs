using Entities;

namespace Services
{
    public interface IProductService
    {
        Task<Product> GetById(int id);
        Task<List<Product>> GetProducts(string? description, int? minPrice, int? maxPrice, int?[] categoriesId, int? position, int? skip);
    }
}