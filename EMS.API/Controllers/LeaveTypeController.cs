using EMS.Core.DTOs.Leave;
using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Data.Repositories;
using EMS.Data.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypeController : BaseController
    {
        private readonly IRepository<Leavetype> _repo;
        private readonly IUnitOfWork _unitOfWork;

        public LeaveTypeController(IRepository<Leavetype> repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leaveTypes = await _repo.TableNoTracking
                .Where(l => l.IsDeleted != true)
                .Select(l => new LeaveTypeResponseDto
                {
                    Id = l.Id,
                    LeaveTypeName = l.LeaveTypeName,
                    DefaultDaysPerYear = l.DefaultDaysPerYear
                })
                .ToListAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Leave types fetched successfully.",
                Data = leaveTypes,
                StatusCode = 200
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR,Employee,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateLeaveTypeDto dto)
        {
            var exists = await _repo.TableNoTracking
                .AnyAsync(l => l.LeaveTypeName == dto.LeaveTypeName && l.IsDeleted != true);

            if (exists)
                throw new InvalidOperationException($"Leave type '{dto.LeaveTypeName}' already exists.");

            var createdBy = GetCurrentUserId();
            var leaveType = new Leavetype
            {
                LeaveTypeName = dto.LeaveTypeName,
                DefaultDaysPerYear = dto.DefaultDaysPerYear,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _repo.AddAsync(leaveType);
            await _unitOfWork.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<object>
            {
                Success = true,
                Message = "Leave type created successfully.",
                Data = new { id = leaveType.Id },
                StatusCode = 201
            });
        }
    }
}
