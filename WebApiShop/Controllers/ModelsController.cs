using Microsoft.AspNetCore.Mvc;
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
                    [FromQuery] int[] categoriesId, [FromQuery] string[] color, int position = 1, int skip = 8)
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
            ModelDTO model = await _modelService.GetModelById(id);
            if(model == null)
                return NotFound();
            return Ok(model);
        }

        // GET api/<DressesController>/10/sizes
        [HttpGet("sizes")]
        public async Task<ActionResult<List<string>>> GetSizesByModelId(int modelId)
        {
            if (await _modelService.GetModelById(modelId) == null)
                return NotFound("not found model with id" + modelId);

            List<string> list = await _modelService.GetSizesByModelId(modelId);
            return Ok(list);
        }

        // GET api/<ModelsController>/5/size/10/date/0000-00-00/count
        [HttpGet("{modelId}/size/{size}/date/{date}/count")]
        public async Task<ActionResult<int>> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date)
        {
            if (await _modelService.GetModelById(modelId) == null)
                return NotFound(" not found model with id" + modelId);
            if (!_modelService.CheckDate(date))
                return BadRequest("the date cant be in the past");

            int count = await _modelService.GetCountByModelIdAndSizeForDate(modelId, size, date);
            return Ok(count);
        }
        // POST api/<ModelsController>
        [HttpPost]
        public async Task<ActionResult<ModelResponseDTO>> AddModel([FromBody] NewModelDTO newModel)
        {
            if (!await _modelService.CheckCategories(newModel.CategoriesId))
                return BadRequest("the caterios not match");
            if (!_modelService.CheckPrice(newModel.BasePrice))
                return BadRequest("the price is not valid");

            ModelResponseDTO model = await _modelService.AddModel(newModel);
            return CreatedAtAction(nameof(GetModelById), new { Id = model.Id }, model);
        }

        // PUT api/<ModelsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModel(int id, [FromBody] NewModelDTO updateModel)
        {
            if (!await _modelService.CheckCategories(updateModel.CategoriesId))
                return BadRequest("the caterios not match");
            if (!_modelService.CheckPrice(updateModel.BasePrice))
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
