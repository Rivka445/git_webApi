using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record NewModelDTO
    (
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
        List<int> CategoriesId
    );
}
