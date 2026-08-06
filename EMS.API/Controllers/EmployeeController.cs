using EMS.Core.DTOs.Employees;
using EMS.Core.Helpers;
using EMS.Service.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace EMS.API.Controllers
{
    [ApiController]
        [Route("api/[controller]")]
        public class EmployeeController : BaseController
        {
            private readonly IEmployeeService _Employeeservice;

            public EmployeeController(IEmployeeService Employeeservice)
            {
                _Employeeservice = Employeeservice;
            }
        [HttpPost("Create-Employee")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
        {
            // ModelState check — REMOVE pannunga, Program.cs-la already handle aagirukku
            var createdBy = GetCurrentUserId();
            var result = await _Employeeservice.CreateEmployeeAsync(dto, createdBy);

            var response = new ApiResponse<EmployeeResponseDto>
            {
                Success = true,
                Message = "Employee profile created successfully.",
                Data = result,
                StatusCode = 201
            };

            return StatusCode(201, response);
        }
        [HttpGet("Get-Employee/{id:int}")]
        [Authorize(Roles = "Admin,HR,Employee,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _Employeeservice.GetEmployeeFullDetailsByIdAsync(id);

            var response = new ApiResponse<EmployeeFullDetailsDto>
            {
                Success = true,
                Message = "Employee details retrieved successfully.",
                Data = result,
                StatusCode = 200
            };
            return Ok(response);
           
        }
        [HttpGet("GetAll-Employee")]
        [Authorize(Roles = "Admin,HR,Employee")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequest request)
        {
            var result = await _Employeeservice.GetAllEmployeesAsync(request);

            var response = new ApiResponse<PagedResult<EmployeeResponseDto>>
            {
                Success = true,
                Message = "Employee details retrieved successfully.",
                Data = result,
                StatusCode = 200
            };
            return Ok(response);

        }

    }
}
