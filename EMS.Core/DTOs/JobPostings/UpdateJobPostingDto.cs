using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core.Enums;
namespace EMS.Core.DTOs.JobPostings
{
    public  class UpdateJobPostingDto
    {

            public string Title { get; set; } = null!;

            public int? DepartmentId { get; set; }

            public decimal? MinSalary { get; set; }

            public decimal? MaxSalary { get; set; }

            public EmploymentType EmploymentType { get; set; } 

            public WorkMode WorkMode { get; set; } 

            public decimal? MinExperience { get; set; }

            public decimal? MaxExperience { get; set; }

            public string? Description { get; set; }

            public string? Requirements { get; set; }

            public string? Location { get; set; }

            public int NumberOfOpenings { get; set; }

            public JobPostingStatus Status { get; set; }

            public DateOnly PostedDate { get; set; }

            public DateOnly? ClosingDate { get; set; }


        }
}
