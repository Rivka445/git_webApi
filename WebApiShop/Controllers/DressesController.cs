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
    public class DressesController : ControllerBase
    {
        private readonly IDressService _dressService;
        private readonly IModelService _modelService;


        public DressesController(IDressService dressService, IModelService modelService)
        {
            _dressService = dressService;
            _modelService = modelService;
        }

        // GET api/<DressesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DressDTO>> GetDressById(int id)
        {
            DressDTO dress = await _dressService.GetDressById(id);
            return dress != null ? Ok(dress) : NotFound();
        }
        // GET api/<DressesController>/sizes
        [HttpGet("sizes")]
        public async Task<ActionResult<List<string>>> GetSizesByModelId(int modelId)
        {
            if (await _modelService.GetModelById(modelId) == null)
                return NotFound(" not founs model wuth id" + modelId);

            List<string> list = await _dressService.GetSizesByModelId(modelId);
            return Ok(list);
        }

        // GET api/<DressesController>/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCountByModelIdAndSizeForDate(int modelId, string size, DateOnly date)
        {
            if (await _modelService.GetModelById(modelId) == null)
                return NotFound(" not founs model with id" + modelId);
            if (!_dressService.checkDate(date))
                return BadRequest("the date cant be in the past");

            int count = await _dressService.GetCountByModelIdAndSizeForDate(modelId, size, date);
            return Ok(count);
        }

        // POST api/<DressesController>
        [HttpPost]
        public async Task<ActionResult<DressDTO>> AddDress([FromBody] NewDressDTO newDress)
        {
            if (!_dressService.checkPrice(newDress.Price))
                return BadRequest("Price must be more than 0.");
            if (!await _modelService.IsExistsModelById(newDress.ModelId))
                return BadRequest("is not model id.");

            DressDTO user = await _dressService.AddDress(newDress);
            return CreatedAtAction(nameof(GetDressById), new { Id = user.Id }, user);
        }

        // PUT api/<DressesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDress(int id, [FromBody] NewDressDTO updateDress)
        {
            if (!_dressService.checkPrice(updateDress.Price))
                return BadRequest("Price must be more than 0.");

            if (!await _dressService.IsExistsDressById(id))
                return NotFound("not found dress with ID " + id);

            if (!await _modelService.IsExistsModelById(updateDress.ModelId))
                return NotFound(" not founs model wuth id" + updateDress.ModelId);


            await _dressService.UpdateDress(id, updateDress);
            return Ok();
        }

        // DELETE api/<DressesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _dressService.IsExistsDressById(id))
                return NotFound("not found dress with ID " + id);
            await _dressService.DeleteDress(id);
            return Ok();
        }
    }
}
