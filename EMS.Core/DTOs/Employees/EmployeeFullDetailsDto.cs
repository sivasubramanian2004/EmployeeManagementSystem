using EMS.Core.DTOs.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Employees
{
    
   

    public class EmployeeFullDetailsDto
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;
        public string EmpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly DateOfJoining { get; set; }
        public string? BloodGroup { get; set; }

        //public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }   // resolved name, not just the FK — useful for a read response

        //public int DesignationId { get; set; }
        public string? DesignationName { get; set; }  // same reasoning

        //public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public PersonalDetailsResponseDto? PersonalDetails { get; set; }

        public List<DocumentResponseDto> Documents { get; set; } = new();
       
    }
}
