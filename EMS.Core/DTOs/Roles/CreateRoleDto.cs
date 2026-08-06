using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Roles
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(100)]
        public string RoleName { get; set; } = null!;
    }
}
