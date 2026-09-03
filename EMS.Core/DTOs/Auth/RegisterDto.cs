using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Auth
{
    public class RegisterDto
    {

        [Required(ErrorMessage ="Name is Required")]
        [StringLength(150)]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage ="Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage ="Password is Required")]
        public string Password { get; set; } = null!;

    }
}
