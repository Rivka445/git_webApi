using Entities;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization; 

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventDressRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserPasswordService _userPasswordService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IUserPasswordService userPasswordService, ILogger<UsersController> logger)
        {
            _logger = logger;
            _userService = userService;
            _userPasswordService = userPasswordService;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            List<UserDTO> users = await _userService.GetUsers();
            if(users.Count()==0)
                return NoContent();
            return Ok(users);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserId(int id)
        {
            UserDTO user = await _userService.GetUserById(id);
            return user != null ? Ok(user) : NotFound();
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<ActionResult<UserDTO>> AddUser([FromBody] UserRegisterDTO newUser)
        {
            int passwordScore = _userPasswordService.CheckPassword(newUser.Password);
            if(passwordScore < 2)
            {
                _logger.LogWarning("Registration failed: weak password for {FirstName} {LastName}", newUser.FirstName, newUser.LastName);
                return BadRequest("Password is not strong enough");
            }
            UserDTO user = await _userService.AddUser(newUser);
            _logger.LogInformation("User registered successfully: {FirstName} {LastName}", user.FirstName, user.LastName);
            return CreatedAtAction(nameof(GetUserId), new { Id = user.Id }, user);
        }
        // POST api/<UsersController>
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> LogIn([FromBody] UserLoginDTO existingUser)
        {
            _logger.LogInformation("Login attempt for {FirstName} {LastName}", existingUser.FirstName, existingUser.LastName);
            UserDTO user = await _userService.LogIn(existingUser);
            if(user == null)
            {
                _logger.LogWarning("Login failed for {FirstName} {LastName}", existingUser.FirstName, existingUser.LastName);
                return Unauthorized("user name or password are wrong");
            }
            _logger.LogInformation("Login succeeded for user {UserId} {Email}", user.Id, user.Email);
            return Ok(user);
        }
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserRegisterDTO updateUser)
        {
            if (await _userService.IsExistsUserById(id) == false)
            {
                _logger.LogWarning("User update failed: user {UserId} not found", id);
                return NotFound(id);
            }
            int passwordScore = _userPasswordService.CheckPassword(updateUser.Password);
            if (passwordScore < 2)
            {
                _logger.LogWarning("User update failed: weak password for user {UserId}", id);
                return BadRequest("Password is not strong enough");
            }
            await _userService.UpdateUser(id, updateUser);
            _logger.LogInformation("User updated successfully: {UserId}", id);
            return Ok();
        }
    }
}
