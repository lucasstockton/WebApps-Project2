using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Matricks.DTOs
{
    public class RegisterUserDTO
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "{0} is required") ]
        public string UserName { get; set; }

        [MinLength(6, ErrorMessage = "{0} must be at least {1} characters long")]
        public string Password { get; set; }
    }
}
