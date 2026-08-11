using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class Employee
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string EmpNo { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateOnly DateOfJoining { get; set; }

    public string? BloodGroup { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public int DepartmentId { get; set; }

    public int DesignationId { get; set; }

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public int? ManagerId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual Designation Designation { get; set; } = null!;

    public virtual ICollection<Employeedocument> Employeedocuments { get; set; } = new List<Employeedocument>();

    public virtual Employeepersonaldetail? Employeepersonaldetail { get; set; }

    public virtual ICollection<Leaverequest> Leaverequests { get; set; } = new List<Leaverequest>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();

    public virtual User User { get; set; } = null!;
}
