using Entities;
using Repositories;
using Services;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace EventDressRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/<CategoriesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            List<CategoryDTO> categories= await _categoryService.GetCategories();
            if (categories.Count() == 0)
                return NoContent();
            return Ok(categories);
        }

        // GET api/<CategoriesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id)
        {
            CategoryDTO categoryDTO = await _categoryService.GetCategoryId(id);
            return categoryDTO != null ? Ok(categoryDTO) : NotFound();
        }

        // POST api/<CategoriesController>
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> AddCategory([FromBody] NewCategoryDTO newCategory)
        {
            CategoryDTO category = await _categoryService.AddCategory(newCategory);
            return CreatedAtAction(nameof(GetCategoryById), new { Id = category.Id }, category);
        }

    }
}
