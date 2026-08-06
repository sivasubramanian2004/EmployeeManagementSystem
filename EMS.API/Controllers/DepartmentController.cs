using EMS.Core.DTOs.Departments;
using EMS.Core.DTOs.Designation;
using EMS.Core.Helpers;
using EMS.Data;
using EMS.Service.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DepartmentController : BaseController
    {
       
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService) => _departmentService = departmentService;

        [Authorize]
        [HttpPost("Add-Departments")]
        public async Task<IActionResult> Insert([FromBody] CreateDepartmentDto dto) {
            var CreatedBy = GetCurrentUserId();
            await _departmentService.InsertAsync(dto, CreatedBy);

            var response = new ApiResponse<Object>
            {
                Success = true,
                Message = "Department Added Successfully",
                Data = null,
                Errors = null,
                StatusCode = 201
            };
            return StatusCode(201, response);
        }


        [Authorize]
        [HttpDelete("Delete-Department/{id:int}")]
        public async Task<IActionResult> Delete(int id) {

            var DeletedBy = GetCurrentUserId();
            await _departmentService.DeleteAsync(id, DeletedBy);
            var response = new ApiResponse<Object>
            {

                Success = true,
                Message = "Department deleted Successfully",
                Data = null,
                Errors = null,
                StatusCode = 200
            };
            return Ok(response);

        }
        [Authorize]
        [HttpPut("update-Department/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateDepartmentDto dto)
        {

            var UpdatedBy = GetCurrentUserId();
            var result =await _departmentService.UpdateAsync(id,dto, UpdatedBy);
            var response = new ApiResponse<Object>
            {

                Success = true,
                Message = "Department Updated Successfully",
                Data =result ,
                Errors = null,
                StatusCode = 200
            };
            return Ok(response);

        }


        [Authorize]
        [HttpGet("Get-Department")]
        public async Task<IActionResult> Getdepartment() {

            var result = await _departmentService.GetAllAsync();

            var response = new ApiResponse<List<DesignationResponseDto>>
            {
                Success = true,
                Message = "Department Fetched Successfully",
                Data=result,
                Errors = null,
                StatusCode = 200

            };
            return Ok(response);

        }



    }

}
