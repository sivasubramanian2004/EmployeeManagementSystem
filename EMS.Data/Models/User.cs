using System;
using System.Collections.Generic;

namespace EMS.Data.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? OtpCode { get; set; }

    public DateTime? OtpExpiryTime { get; set; }

    public bool? OtpIsUsed { get; set; }

    public virtual Employee? Employee { get; set; }
}
