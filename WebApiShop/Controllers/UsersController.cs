using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        string _filePath = "D:\\webAPI\\WebApiShop\\WebApiShop\\users.txt";

        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "flight1", "flight2" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{Id}")]
        public ActionResult<User> GetId(int id)
        {
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User user = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (user!=null && user.Id == id)
                        return user;
                }
            }
            return new User();
        }

        // POST api/<UsersController>
        [HttpPost]
        public ActionResult<User> Post([FromBody] User newUser)
        {
            int numberOfUsers = System.IO.File.ReadLines(_filePath).Count();
            newUser.Id = numberOfUsers + 1;
            string userJson = JsonSerializer.Serialize(newUser);
            System.IO.File.AppendAllText(_filePath, userJson + Environment.NewLine);
            return CreatedAtAction(nameof(Get), new { Id=newUser.Id},newUser);
        }

        // POST api/<UsersController>
        [HttpPost("{login}")]
        public ActionResult<ExistUser> LogIn([FromBody] ExistUser existUser)
        {
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string? currentUserInFile;
                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User userFromFile = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (userFromFile != null && userFromFile.UserName == existUser.UserName && userFromFile.Password == existUser.Password)
                        return CreatedAtAction(nameof(Get), new { id = userFromFile.Id }, userFromFile);
                }
            }
            return NotFound();
        }
        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User updateUser)
        {
            string textToReplace = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText(_filePath))
            {
                string currentUserInFile;

                while ((currentUserInFile = reader.ReadLine()) != null)
                {
                    User userFromFile = JsonSerializer.Deserialize<User>(currentUserInFile);
                    if (userFromFile != null && userFromFile.Id == id)
                        textToReplace = currentUserInFile;
                }
            }
            if (textToReplace != string.Empty)
            {
                string text = System.IO.File.ReadAllText(_filePath);
                text = text.Replace(textToReplace, JsonSerializer.Serialize(updateUser));
                System.IO.File.WriteAllText(_filePath, text);
            }
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
