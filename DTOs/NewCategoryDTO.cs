using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record NewCategoryDTO
    (
        [Required]
        int Id,
        [Required]
        string Name,
        string Description
    );
}
