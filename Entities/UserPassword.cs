using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserPassword
    {

        [StringLength( 200,MinimumLength =2 ), Required]
        public string Password { get; set; } = "";
        
    }
}
