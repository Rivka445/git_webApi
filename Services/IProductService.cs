using Entities;
using DTOs;
namespace Services
{
    public interface IProductService
    {
        Task<ProductDTO> GetById(int id);
        Task<List<ProductDTO>> GetProducts(string? description, int? minPrice, int? maxPrice, int?[] categoriesId, int? position, int? skip);
    }
}