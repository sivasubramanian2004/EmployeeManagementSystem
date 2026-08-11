using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Career
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? Mobile { get; set; }

    public string? Street { get; set; }

    public string City { get; set; } = null!;

    public string Pincode { get; set; } = null!;

    public string? State { get; set; }

    public string? Country { get; set; }

    public string JobApplicationPosition { get; set; } = null!;

    public decimal Experience { get; set; }

    public string? CurrentDesignation { get; set; }

    public string? CurrentCompany { get; set; }

    public string? CurrentSalary { get; set; }

    public decimal? ExpectedSalary { get; set; }

    public string? NoticePeriod { get; set; }

    public string SkillSet { get; set; } = null!;

    public string? LinkedInUrl { get; set; }

    public string? GitHubUrl { get; set; }

    public string? PhotoPath { get; set; }

    public string ResumePath { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? ReferralEmail { get; set; }

    public int? JobPostingId { get; set; }

    public virtual ICollection<Careereducation> Careereducations { get; set; } = new List<Careereducation>();

    public virtual Jobposting? JobPosting { get; set; }
}
