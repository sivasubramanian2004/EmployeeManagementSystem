using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Employees
{
    public class PersonalDetailsResponseDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        public string? MaritalStatus { get; set; }
        public string? Nationality { get; set; }

        // Masked by default — see note below
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
    }
}
