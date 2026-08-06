using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Core.DTOs.Leave
{
    public class CreateLeaveTypeDto
    {
        public string LeaveTypeName { get; set; } = string.Empty;
        public int DefaultDaysPerYear { get; set; }
    }

    public class LeaveTypeResponseDto
    {
        public int Id { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty
        public int DefaultDaysPerYear { get; set; }
    }
}
