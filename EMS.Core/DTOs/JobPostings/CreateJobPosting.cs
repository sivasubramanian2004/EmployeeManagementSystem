using EMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.JobPostings
{
    public class CreateJobPostingDto
    {
        [Required(ErrorMessage = "Job Title is Required")]
        [StringLength(150)]
        public string Title { get; set; } = null!;
        public int? DepartmentId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MinSalary cannot be negative.")]
        public decimal? MinSalary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxSalary cannot be negative.")]
        public decimal? MaxSalary { get; set; }

        [Required(ErrorMessage = "EmploymentType is Required")]
        public EmploymentType EmploymentType { get; set; }

        [Required(ErrorMessage = "WorkMode is Required")]
        public WorkMode WorkMode { get; set; }

        [Range(0, 100, ErrorMessage = "MinExperience must be between 0 and 100 years.")]
        public decimal? MinExperience { get; set; }

        [Range(0, 100, ErrorMessage = "MaxExperience must be between 0 and 100 years.")]
        public decimal? MaxExperience { get; set; }

        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
        public string? Description { get; set; }

        [StringLength(3000, ErrorMessage = "Requirements cannot exceed 3000 characters.")]
        public string? Requirements { get; set; }

        [StringLength(150)]
        public string? Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Number of Openings must be at least 1.")]
        public int NumberOfOpenings { get; set; }

        [Required(ErrorMessage = "PostedDate is Required")]
        public DateOnly PostedDate { get; set; }

        public DateOnly? ClosingDate { get; set; }
    }
}
