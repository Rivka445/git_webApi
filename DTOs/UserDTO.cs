using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record UserDTO
    (
        [Required]
        int Id,
        [Required]
        string FirstName,
        [Required]
        string LastName,
        [EmailAddress]
        string Email,
        [Required,Phone]
        string Phone
    );  
}
