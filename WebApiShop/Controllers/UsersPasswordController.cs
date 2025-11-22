using Microsoft.AspNetCore.Mvc;
using Repositories;
using Entities;
using Services;
using System.Collections.Generic;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersPasswordController : ControllerBase
    {
        private readonly IUserPasswordService _userPasswordService;

        public UsersPasswordController(IUserPasswordService userPasswordService)
        {
            _userPasswordService = userPasswordService;
        }
        // GET: api/<UsersPasswordController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsersPasswordController>/5
        [HttpGet("{id}")]
        public string Get(UserPassword password)
        {
            return "value";
        }
        
        // POST api/<UsersPasswordController>
        [HttpPost]
        public ActionResult<int> CheckPassword([FromBody] UserPassword password)
        {
            int score = _userPasswordService.CheckPassword(password.Password);
            if (score > 1)
                return Ok(score);
            return BadRequest("Password is not strong enough");
        }
        
        // PUT api/<UsersPasswordController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersPasswordController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
