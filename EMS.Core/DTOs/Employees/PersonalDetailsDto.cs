using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Employees
{
    public class PersonalDetailsDto
    {
        [RegularExpression("Single|Married|Divorced|Widowed", ErrorMessage = "Invalid marital status.")]
        public string? MaritalStatus { get; set; }

        [StringLength(50)]
        public string? Nationality { get; set; }

       // [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Aadhar number format must be XXXX-XXXX-XXXX.")]
        public string? AadharNumber { get; set; }

       // [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format.")]
        public string? PanNumber { get; set; }

        [StringLength(150)]
        public string? EmergencyContactName { get; set; }

        [Phone]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(50)]
        public string? EmergencyContactRelation { get; set; }

        [StringLength(30)]
        public string? BankAccountNumber { get; set; }

        [StringLength(100)]
        public string? BankName { get; set; }

        [StringLength(20)]
        public string? IfscCode { get; set; }

        [StringLength(100)]
        public string? District { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be 6 digits.")]
        public string? Pincode { get; set; }

        [StringLength(150)]
        public string? FatherName { get; set; }

        [StringLength(100)]
        public string? FatherOccupation { get; set; }

        [Phone]
        public string? FatherMobileNo { get; set; }

        [StringLength(150)]
        public string? MotherName { get; set; }

        [StringLength(100)]
        public string? MotherOccupation { get; set; }

        [Range(0, 20)]
        public int? SiblingCount { get; set; }

        [Range(0, 99999999.99)]
        public decimal? FamilyIncome { get; set; }

        public bool? IsFirstGraduate { get; set; }

        [StringLength(200)]
        public string? SslcSchoolName { get; set; }

        [Range(0, 100)]
        public decimal? SslcPercentage { get; set; }

        [StringLength(200)]
        public string? HscOrDiplomaSchoolName { get; set; }

        [Range(0, 100)]
        public decimal? HscOrDiplomaPercentage { get; set; }

        [StringLength(200)]
        public string? UgCollegeName { get; set; }

        [Range(0, 10)]
        public decimal? UgCgpa { get; set; }

        [StringLength(200)]
        public string? PgDegreeCollegeName { get; set; }

        [Range(0, 10)]
        public decimal? PgCgpa { get; set; }
    }
}
