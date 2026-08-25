using EMS.Core.DTOs.Leave;
using EMS.Core.Helpers;
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

        Task<LeaveRequestResponseDto> GetLeavesAsync(int id);
        Task<LeaveRequestResponseDto> ApproveLeaveAsync(int leaveRequestId, int managerEmployeeId, UpdateleaveDto dto,int UpdatedBy);

        Task<PagedResult<LeaveRequestResponseDto>> GetAllAsync(
           LeaveFilterRequestDto request,
           int managerEmployeeId);


    }
}
