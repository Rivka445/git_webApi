using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record NewOrderDTO
    (
        [Required]
        DateOnly OrderDate,
        [Required]
        DateOnly EventDate,
        [Required]
        int FinalPrice,
        [Required]
        int UserId,
        string Note,
        List<NewOrderItemDTO> OrderItems
    );
}
