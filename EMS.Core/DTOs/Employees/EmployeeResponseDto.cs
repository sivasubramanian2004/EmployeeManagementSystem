using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Employees
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }
        public string EmpNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public DateOnly DateOfJoining { get; set; }

       // public bool HasPersonalDetails { get; set; }
    }
}
