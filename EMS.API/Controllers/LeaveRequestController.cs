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

    [HttpPost("Apply-Leave")]
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


     [Authorize(Roles ="Manager, HR")]
     [HttpGet("Get-leaves/{id:int}")]  
      public async Task<IActionResult> GetLeaves(int id)
      {
         // var employeeId = await GetCurrentEmployeeIdAsync();
          var result = await _service.GetLeavesAsync(id);

          return Ok(new ApiResponse<object>
          {
              Success = true,
              Message = "leave requests fetched successfully.",
              Data = result,
              StatusCode = 200
          });
      }
  
  
    [HttpPut("approve{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Approve(int id, UpdateleaveDto dto )
    {
        var managerEmployeeId = await GetCurrentEmployeeIdAsync();
        var UpdatedBy = GetCurrentUserId();
        var result = await _service.ApproveLeaveAsync(managerEmployeeId,id, dto,UpdatedBy);

        return Ok(new ApiResponse<LeaveRequestResponseDto>
        {
            Success = true,
            Message = "Leave request approved successfully.",
            Data = result,
            StatusCode = 200
        });
    }

    [HttpGet]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetAll([FromQuery] LeaveFilterRequestDto request)
    {
        var managerEmployeeId = await GetCurrentEmployeeIdAsync();

        var result = await _service.GetAllAsync(request, managerEmployeeId);
        return Ok(new ApiResponse<PagedResult<LeaveRequestResponseDto>>
        {
            Success = true,
            Message = "Leave requests retrieved successfully.",
            Data = result,
            StatusCode = 200
        });
    }



}