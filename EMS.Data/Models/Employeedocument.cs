using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Employeedocument
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string DocumentType { get; set; } = null!;

    public string DocumentName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int? FileSize { get; set; }

    public DateTime UploadedDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
