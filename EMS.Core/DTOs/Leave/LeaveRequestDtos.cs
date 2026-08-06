using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EMS.Core.DTOs.Leave
{
   
 

    public class ApplyLeaveDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Valid LeaveTypeId is required.")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }

    public class RejectLeaveDto
    {
        [Required(ErrorMessage = "Rejection reason is required.")]
        [StringLength(500, MinimumLength = 5)]
        public string RejectionReason { get; set; } = string.Empty;
    }

    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string LeaveTypeName { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int TotalDays { get; set; }

        public string? Reason { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ApprovedByName { get; set; }

        public DateTime? ActionDate { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
