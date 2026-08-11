using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Careers
{
    public class CareerEducationDto
    {
        [Required]
        public string EducationType { get; set; } = string.Empty;

        [Required]
        public string InstitutionName { get; set; } = string.Empty;

        [Required]
        public string Degree { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal Percentage { get; set; }

        [Required]
        public DateOnly FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
    }
}
