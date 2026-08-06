using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Departments
{
    internal class DepartmentResponseDto
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
