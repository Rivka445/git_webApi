using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class OrderItemDTO
    {
        public OrderItemDTO() { }

        [Required]
        public int DressId { get; set; }

        [Required]
        public int DressPrice { get; set; }

        [Required]
        public string ModelName { get; set; }
    }
    
}
