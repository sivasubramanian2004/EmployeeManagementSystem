using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Leavetype
{
    public int Id { get; set; }

    public string LeaveTypeName { get; set; } = null!;

    public int DefaultDaysPerYear { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Leaverequest> Leaverequests { get; set; } = new List<Leaverequest>();
}
