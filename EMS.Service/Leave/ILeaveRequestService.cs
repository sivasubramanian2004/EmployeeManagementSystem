using EMS.Core.DTOs.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Leave
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestResponseDto> ApplyLeaveAsync(int employeeId, ApplyLeaveDto dto);
        //Task<List<LeaveRequestResponseDto>> GetMyLeavesAsync(int employeeId);
        Task<List<LeaveRequestResponseDto>> GetPendingApprovalsAsync(int managerEmployeeId);
        Task<LeaveRequestResponseDto> ApproveLeaveAsync(int leaveRequestId, int managerEmployeeId);
        Task<LeaveRequestResponseDto> RejectLeaveAsync(int leaveRequestId, int managerEmployeeId, RejectLeaveDto dto);
    }
}
