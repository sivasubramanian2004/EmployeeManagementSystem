using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Departments
{
    public class CreateDepartmentDto

    {
        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = null!;


    }
}
