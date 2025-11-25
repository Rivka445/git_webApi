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
        IUserPasswordService _userPasswordService;

        public UsersPasswordController(IUserPasswordService userPassword)
        {
            _userPasswordService=userPassword;
        }

        

        // POST api/<UsersPasswordController>
        [HttpPost]
        public ActionResult<int>CheckPassword([FromBody] UserPassword password)
        {
            int score = _userPasswordService.CheckPassword(password.Password);
            if (score > 1)
                return Ok(score);
            return BadRequest();
        }
    }
}
