using System.ComponentModel.DataAnnotations;

namespace WebApiShop.Controllers
{
    public class User
    {
        [EmailAddress, Required]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }

        [StringLength(8, ErrorMessage = "password Can be between 4 till 8 chars", MinimumLength = 4), Required]
        public string Password { get; set; }

    }
    public class ExistUser
    {
        [EmailAddress, Required]
        public string UserName { get; set; }

        [StringLength(8, ErrorMessage = "password Can be between 4 till 8 chars", MinimumLength = 4), Required]
        public string Password { get; set; }

    }
}