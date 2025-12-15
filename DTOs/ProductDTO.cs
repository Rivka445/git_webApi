using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record ProductDTO
    (
        int Id,
        [Required]
        string Name,
        string CategoryName,
        string Description,
        [Required]
        double Price,
        string ImgUrl
    );
}
