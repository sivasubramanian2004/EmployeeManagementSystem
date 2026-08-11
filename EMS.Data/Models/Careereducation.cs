using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Careereducation
{
    public int Id { get; set; }

    public int CareerId { get; set; }

    public string EducationType { get; set; } = null!;

    public string InstitutionName { get; set; } = null!;

    public string Degree { get; set; } = null!;

    public decimal Percentage { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Career Career { get; set; } = null!;
}
