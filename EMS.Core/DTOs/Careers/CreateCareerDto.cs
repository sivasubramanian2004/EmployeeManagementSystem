using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace EMS.Core.DTOs.Careers;


public class CreateCareerDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Mobile { get; set; }

    [StringLength(250)]
    public string? Street { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Pincode { get; set; } = string.Empty;

    public string? State { get; set; }

    public string? Country { get; set; }

    [Required]
    public string JobApplicationPosition { get; set; } = string.Empty;

    public string?  ReferralEmail { get; set; }


    [Required]
    [Range(0, 50)]
    public decimal Experience { get; set; }

    public string? CurrentDesignation { get; set; }

    public string? CurrentCompany { get; set; }

    [Range(0, double.MaxValue)]
    public string? CurrentSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ExpectedSalary { get; set; }

    public string? NoticePeriod { get; set; }

    [Required]
    public string SkillSet { get; set; } = string.Empty;

    [Url]
    public string? LinkedInUrl { get; set; }

    [Url]
    public string? GitHubUrl { get; set; }

    public IFormFile? Photo { get; set; }

    [Required]
    public IFormFile Resume { get; set; } = null!;

    public List<CareerEducationDto> Educations { get; set; } = new();
}

