using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record OrderDTO
    (
        [Required]
        int Id,
        [Required]
        DateOnly Date,
        [Required]
        int Sum,
        List<OrderItemDTO> OrderItems
    );
}
