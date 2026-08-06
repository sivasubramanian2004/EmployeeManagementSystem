using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Employeepersonaldetail
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Nationality { get; set; }

    public string? AadharNumber { get; set; }

    public string? PanNumber { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public string? EmergencyContactRelation { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankName { get; set; }

    public string? IfscCode { get; set; }

    public string? District { get; set; }

    public string? State { get; set; }

    public string? Pincode { get; set; }

    public string? FatherName { get; set; }

    public string? FatherOccupation { get; set; }

    public string? FatherMobileNo { get; set; }

    public string? MotherName { get; set; }

    public string? MotherOccupation { get; set; }

    public int? SiblingCount { get; set; }

    public decimal? FamilyIncome { get; set; }

    public bool? IsFirstGraduate { get; set; }

    public string? SslcSchoolName { get; set; }

    public decimal? SslcPercentage { get; set; }

    public string? HscOrDiplomaSchoolName { get; set; }

    public decimal? HscOrDiplomaPercentage { get; set; }

    public string? UgCollegeName { get; set; }

    public decimal? UgCgpa { get; set; }

    public string? PgDegreeCollegeName { get; set; }

    public decimal? PgCgpa { get; set; }

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
