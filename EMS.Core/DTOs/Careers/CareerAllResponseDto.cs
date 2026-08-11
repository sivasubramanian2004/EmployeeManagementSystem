using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Careers
{
    public class CareerAllResponseDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Mobile { get; set; }

        public string? Street { get; set; }

        public string City { get; set; } = string.Empty;

        public string Pincode { get; set; } = string.Empty;

        public string? State { get; set; }

        public string? Country { get; set; }

        public string JobApplicationPosition { get; set; } = string.Empty;

        public decimal Experience { get; set; }

        public string? CurrentDesignation { get; set; }

        public string? CurrentCompany { get; set; }

        public string? CurrentSalary { get; set; }

        public decimal? ExpectedSalary { get; set; }

        public string? NoticePeriod { get; set; }

        public string SkillSet { get; set; } = string.Empty;

        public string? LinkedInUrl { get; set; }

        public string? GitHubUrl { get; set; }

        // Full URLs
        public string? PhotoUrl { get; set; }

        public string? ResumeUrl { get; set; }



        // One-to-many
        public List<CareerEducationResponseDto> Educations { get; set; }
            = new();
    }


    public class CareerEducationResponseDto
    {
        public int Id { get; set; }

        public string EducationType { get; set; } = string.Empty;

        public string InstitutionName { get; set; } = string.Empty;

        public string Degree { get; set; } = string.Empty;

        public decimal Percentage { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
    }



    public class CareerCandidatedata
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Mobile { get; set; }
        public string City { get; set; } = string.Empty;
        public string JobApplicationPosition { get; set; } = string.Empty;

        public decimal Experience { get; set; }

        public decimal? ExpectedSalary { get; set; }

        public DateTime AppliedDate { get; set; }

    }
}