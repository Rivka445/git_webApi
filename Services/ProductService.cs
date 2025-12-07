using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Entities;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<Product>> GetProducts(string? description, int? minPrice, int? maxPrice,
            int?[] categoriesId, int? position, int? skip)
        {
            return await _productRepository.GetProducts(description, minPrice, maxPrice, categoriesId, position, skip);
        }
        public async Task<Product> GetById(int id)
        {
            return await _productRepository.GetById(id);
        }
    }
}
