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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService; 
        }

        // GET: api/<UsersController>
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            List <User> users= _userService.GetUsers();
            if(users.Count()==0)
                return NoContent();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        [HttpGet("{Id}")]
        public ActionResult<User> GetId(int id)
        {
            User user = _userService.GetUserById(id);
            return user != null ? Ok(user) : NotFound();
        }

        // POST api/<UsersController>
        [HttpPost]
        public ActionResult<User> Post([FromBody] User newUser)
        {
            User user=_userService.AddUser(newUser);
            return CreatedAtAction(nameof(Get), new { Id= user.Id}, user);
        }

        // POST api/<UsersController>
        [HttpPost("{login}")]
        public ActionResult<User> LogIn([FromBody] User existUser)
        {
            User user = _userService.LogIn(existUser);
            if(user==null)
                return NotFound();
            return CreatedAtAction(nameof(Get), new { Id = user.Id }, user);
        }
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User updateUser)
        {
            _userService.UpdateUser(id, updateUser);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
