using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Jobposting
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

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Career> Careers { get; set; } = new List<Career>();

    public virtual Department? Department { get; set; }
}
