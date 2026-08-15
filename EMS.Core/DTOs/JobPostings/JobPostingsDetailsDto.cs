using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.JobPostings
{
    public class JobPostingsDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int? DepartmentId { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public string EmploymentType { get; set; } = null!;

        public string WorkMode { get; set; } = null!;

        public decimal? MinExperience { get; set; }

        public decimal? MaxExperience { get; set; }

        public string? Description { get; set; }

        public string? Requirements { get; set; }

        public string? Location { get; set; }

        public int NumberOfOpenings { get; set; }

        public string Status { get; set; } = null!;

        public DateOnly PostedDate { get; set; }

        public DateOnly? ClosingDate { get; set; }


    }
}
