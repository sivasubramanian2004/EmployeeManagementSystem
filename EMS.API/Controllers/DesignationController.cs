using EMS.Core.DTOs.Designation;
using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Service.Designations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Security.Claims;

namespace EMS.API.Controllers
{ 
    [ApiController]
    [Route("api/[Controller]")]

    public class DesignationController: BaseController 

    {

        private readonly IDesignationService _designationService;
        public DesignationController(IDesignationService DesignationService) { _designationService = DesignationService; }
        
        [Authorize]
        [HttpGet("Get-Designations")]
         public async Task<IActionResult> GetAll()
         {

            var result = await _designationService.GetAllAsync();
           

             return Ok(new ApiResponse<object>
             {
                 Success = true,
                 Message = "Designations fetched successfully",
                 Data = result,
                 StatusCode = 200
             });
         }
        
        [Authorize]
        [HttpPost("Add-Designations")]
        public async Task<IActionResult> Insert([FromBody] CreateDesignationDto dto)

        {
            var createdBy = GetCurrentUserId();
            await _designationService.InsertAsync(dto, createdBy);

            var response = new ApiResponse<Object>
            {
                Success = true,
                Message = $"Designation created successfully. {dto.DesignationName}",
                Data = null,
                StatusCode = StatusCodes.Status201Created
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [Authorize]
        [HttpPut("Update-Designations/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody]CreateDesignationDto dto) {

            var updatedBy = GetCurrentUserId();
            var result = await _designationService.UpdateAsync(id, dto, updatedBy);
            var response=new ApiResponse<DesignationResponseDto> { 
             Success=true,
             Message="Designation Updated Successfully",
             Data=result,
             StatusCode=200
            };
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("Delete-Designations/{id:int}")]
        public async Task<IActionResult> Delete(int id) {

            var deletedBy = GetCurrentUserId();
            await _designationService.DeleteAsync(id,deletedBy);

            var response = new ApiResponse<Object> {
                Success = true,
                Message = "Designation Deleted Successfully.",
                Data = null,
                StatusCode = 200

            };
            return Ok(response);

        }
    }
}
