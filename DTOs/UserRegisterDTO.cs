using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DTOs
{
  public record UserRegisterDTO
  (
        [Required]
        string FirstName,
        [Required]
        string LastName,
        [EmailAddress]
        string Email,
        [Phone,Required]
        string Phone,
        [Required]
        string Password,
        [Required]
        string Role
   );
}
