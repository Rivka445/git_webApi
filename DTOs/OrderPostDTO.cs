using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderPostDTO
    (
        [Required]
        DateOnly Date,
        [Required]
        int Sum,
        [Required]
        int UserId,
        [Required]
        List<OrderItemDTO> OrderItems
    );
}
