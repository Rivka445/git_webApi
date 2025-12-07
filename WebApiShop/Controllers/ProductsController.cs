using Microsoft.AspNetCore.Mvc;
using Entities;
using Repositories;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<List<Product>> Get(string? description, int? minPrice, int? maxPrice,[FromQuery] int?[] categoriesId, int? position, int? skip)
        {
            return await _productService.GetProducts(description, minPrice, maxPrice, categoriesId, position, skip);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<Product> Get(int id)
        {
            return await _productService.GetById(id);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
