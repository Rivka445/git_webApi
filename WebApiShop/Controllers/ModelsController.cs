using Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using DTOs;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventDressRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly IModelService _modelService;


        public ModelsController(IModelService modelService)
        {
            _modelService = modelService;
        }
        // GET: api/<ModelsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinalModels>>> Get(string? description, int? minPrice, int? maxPrice,
                    [FromQuery] int[] categoriesId, string? color, int position = 1, int skip = 8)
        {
            if (!_modelService.ValidateQueryParameters(position, skip, minPrice, maxPrice))
                return BadRequest("is not valid parameters");

            FinalModels products = await _modelService.GetModelds(description, minPrice, maxPrice, categoriesId, color, position, skip);
            if (products.Items.Count() == 0)
                return NoContent();
            return Ok(products);
        }

        // GET api/<ModelsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelDTO>> GetModelById(int id)
        {
            ModelDTO product = await _modelService.GetModelById(id);
            return product != null ? Ok(product) : NotFound();
        }

        // POST api/<ModelsController>
        [HttpPost]
        public async Task<ActionResult<ModelDTO>> AddModel([FromBody] NewModelDTO newModel)
        {
            if (!await _modelService.checkCategories(newModel.CategoriesId))
                return BadRequest("the caterios not match");
            if (!_modelService.checkPrice(newModel.BasePrice))
                return BadRequest("the price is not valid");

            ModelDTO model = await _modelService.AddModel(newModel);
            return CreatedAtAction(nameof(GetModelById), new { Id = model.Id }, model);
        }

        // PUT api/<ModelsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModel(int id, [FromBody] NewModelDTO updateModel)
        {
            if (!await _modelService.checkCategories(updateModel.CategoriesId))
                return BadRequest("the caterios not match");
            if (!_modelService.checkPrice(updateModel.BasePrice))
                return BadRequest("Price must be more than 0.");
            if (!await _modelService.IsExistsModelById(id))
                return NotFound(id);

            await _modelService.UpdateModel(id, updateModel);
            return Ok(); 
        }

        // DELETE api/<ModelsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _modelService.IsExistsModelById(id))
                return NotFound(id);
            await _modelService.DeleteModel(id);
            return Ok();
        }
    }
}
