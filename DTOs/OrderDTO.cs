using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DTOs
{
    public record OrderDTO
    {
        [Required]
        public int Id { get; init; }

        [Required]
        public DateOnly OrderDate { get; init; }

        [Required]
        public DateOnly EventDate { get; init; }

        [Required]
        public int FinalPrice { get; init; }

        [Required]
        public int UserId { get; init; }

        public string? Note { get; init; }
        [Required]
        public int StatusId   { get; init; } = 1!;
        [Required]
        public string StatusName { get; init; } = null!;

        [Required]
        public string UserFirstName { get; init; } = null!;

        [Required]
        public string UserLastName { get; init; } = null!;

        [Required]
        public List<OrderItemDTO> OrderItems { get; init; } = new();
    }
}

