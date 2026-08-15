using EMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.JobPostings
{
    public class JobPostingsfilterDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? DepartmentName { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public string? EmploymentType { get; set; }

        public string? WorkMode { get; set; }

        public decimal? MinExperience { get; set; }

        public decimal? MaxExperience { get; set; }

        public string? Location { get; set; }

        public int NumberOfOpenings { get; set; }

        public string? Status { get; set; }

        public DateOnly PostedDate { get; set; }

        public DateOnly? ClosingDate { get; set; }
    }
}
