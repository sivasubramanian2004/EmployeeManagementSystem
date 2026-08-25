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
    [Required(ErrorMessage ="FirstName is Required")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage ="LastName is Required")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Required(ErrorMessage ="Email Address is Required")]
    [EmailAddress(ErrorMessage ="Invalid Email Address")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Mobile { get; set; }

    [StringLength(250)]
    public string? Street { get; set; }

    [Required(ErrorMessage ="City is Required")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage ="Pincode is Required")]
    [StringLength(20)]
    public string Pincode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? State { get; set; }
    [StringLength(100)]
    public string? Country { get; set; }

    [Required(ErrorMessage ="JobPostion is Required")]
    public int JobPostingId { get; set; }

    [EmailAddress(ErrorMessage ="Invalid Email Address")]
    [StringLength(0)]
    public string?  ReferralEmail { get; set; }


    [Required(ErrorMessage ="Experience is Required")]
    [Range(0, 50)]
    public decimal Experience { get; set; }

    [StringLength(150)]
    public string? CurrentDesignation { get; set; }

    [StringLength(150)]
    public string? CurrentCompany { get; set; }
    [StringLength(50)]
    public string? CurrentSalary { get; set; }

    [StringLength(50)]
    public string? ExpectedSalary { get; set; }
    [StringLength(50)]
    public string? NoticePeriod { get; set; }

    [Required(ErrorMessage ="Skillset are Required")]
    [StringLength(5000)]
    public string SkillSet { get; set; } = string.Empty;

    [Url]
    public string? LinkedInUrl { get; set; }

    [Url]
    public string? GitHubUrl { get; set; }

    public IFormFile? Photo { get; set; }

    [Required(ErrorMessage ="Resume is Required")]
    public IFormFile Resume { get; set; } = null!;

    public List<CareerEducationDto> Educations { get; set; } = new();
}

