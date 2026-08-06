using EMS.Core.DTOs.Roles;
using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Service.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Security.Claims;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]

    public class RoleController : BaseController

    {

        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService) { _roleService = roleService; }

        [Authorize]
        [HttpGet("Get-Roles")]
        public async Task<IActionResult> GetAll()
        {

            var result = await _roleService.GetAllAsync();


            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Roles fetched successfully",
                Data = result,
                StatusCode = 200
            });
        }

        [Authorize]
        [HttpPost("Add-Roles")]
        public async Task<IActionResult> Insert([FromBody] CreateRoleDto dto)

        {
            var createdBy = GetCurrentUserId();
            await _roleService.InsertAsync(dto, createdBy);

            var response = new ApiResponse<Object>
            {
                Success = true,
                Message = $"Role created successfully.",
                Data = null,
                StatusCode = StatusCodes.Status201Created
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize]
        [HttpPut("Update-Roles/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateRoleDto dto)
        {

            var updatedBy = GetCurrentUserId();
            var result = await _roleService.UpdateAsync(id, dto, updatedBy);
            var response = new ApiResponse<RoleResponseDto>
            {
                Success = true,
                Message = "Role Updated Successfully",
                Data = result,
                StatusCode = 200
            };
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("Delete-Roles/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {

            var deletedBy = GetCurrentUserId();
            await _roleService.DeleteAsync(id, deletedBy);

            var response = new ApiResponse<Object>
            {
                Success = true,
                Message = "Role Deleted Successfully.",
                Data = null,
                StatusCode = 200

            };
            return Ok(response);

        }
    }
}
