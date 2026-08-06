using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Core.DTOs.Leave;
using EMS.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Service.Leave;
namespace EMS.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class LeaveRequestController : BaseController
{
    private readonly ILeaveRequestService _service;
    private readonly IRepository<Employee> _employeeRepo;

    public LeaveRequestController(ILeaveRequestService service, IRepository<Employee> employeeRepo)
    {
        _service = service;
        _employeeRepo = employeeRepo;
    }

    private async Task<int> GetCurrentEmployeeIdAsync()
    {
        var userId = GetCurrentUserId();
        var employee = await _employeeRepo.TableNoTracking
            .FirstOrDefaultAsync(e => e.UserId == userId && e.IsDeleted != true)
            ?? throw new KeyNotFoundException("No employee profile linked to your account.");
        return employee.Id;
    }

    [HttpPost("apply")]
    [Authorize]
    public async Task<IActionResult> Apply([FromBody] ApplyLeaveDto dto)
    {
        var employeeId = await GetCurrentEmployeeIdAsync();
        var result = await _service.ApplyLeaveAsync(employeeId, dto);

        return StatusCode(201, new ApiResponse<LeaveRequestResponseDto>
        {
            Success = true,
            Message = "Leave request submitted successfully.",
            Data = result,
            StatusCode = 201
        });
    }

    [HttpGet("my-leaves")]
    [Authorize]
    public async Task<IActionResult> GetMyLeaves()
    {
        var employeeId = await GetCurrentEmployeeIdAsync();
        var result = await _service.GetMyLeavesAsync(employeeId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Your leave requests fetched successfully.",
            Data = result,
            StatusCode = 200
        });
    }

    [HttpGet("pending-approvals")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetPendingApprovals()
    {
        var managerEmployeeId = await GetCurrentEmployeeIdAsync();
        var result = await _service.GetPendingApprovalsAsync(managerEmployeeId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Pending approvals fetched successfully.",
            Data = result,
            StatusCode = 200
        });
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Approve(int id)
    {
        var managerEmployeeId = await GetCurrentEmployeeIdAsync();
        var result = await _service.ApproveLeaveAsync(id, managerEmployeeId);

        return Ok(new ApiResponse<LeaveRequestResponseDto>
        {
            Success = true,
            Message = "Leave request approved successfully.",
            Data = result,
            StatusCode = 200
        });
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectLeaveDto dto)
    {
        var managerEmployeeId = await GetCurrentEmployeeIdAsync();
        var result = await _service.RejectLeaveAsync(id, managerEmployeeId, dto);

        return Ok(new ApiResponse<LeaveRequestResponseDto>
        {
            Success = true,
            Message = "Leave request rejected successfully.",
            Data = result,
            StatusCode = 200
        });
    }
}