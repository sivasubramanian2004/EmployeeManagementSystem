using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Designation
{
    public class CreateDesignationDto
    {
        [Required]
        [StringLength(100)]
        public string DesignationName { get; set; }= string.Empty;
    }
}
