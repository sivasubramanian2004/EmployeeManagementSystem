using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Salary
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public decimal BasicPay { get; set; }

    public decimal Allowances { get; set; }

    public decimal Deductions { get; set; }

    public decimal NetPay { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
