using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record ModelDTO
    (
        [Required]
        int Id,
        [Required]
        string Name,
        [Required]
        string Description,
        [Required]
        string ImgUrl,
        [Required]
        int BasePrice,
        [Required]
        string Color,
        [Required]
        bool IsActive,
        [Required]
        List<CategoryDTO> Categories
    );
}
