
using EMS.Core.DTOs.Careers;
using EMS.Core.DTOs.Employees;
using EMS.Core.Helpers;
using EMS.Service.Careers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EMS.API.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CareerController : BaseController
    {
        private readonly ICareerService _careerService;

        public CareerController(ICareerService careerService)
        {
            _careerService = careerService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCareerDto dto)
        {
            var result = await _careerService.CreateAsync(dto);

        var response = new ApiResponse<CareerResponseDto>
        {
            Success = true,
            Message = "Application submitted successfully.",
            Data = result,
            Errors = null,
            StatusCode = 201
        };
        return StatusCode(201, response);
    }


    [HttpGet("Get-Candidates/{id:int}")]
    [Authorize(Roles = "Admin,HR,Employee,Manager")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _careerService.GetCandidateDetailsByIdAsync(id);

        var response = new ApiResponse<CareerAllResponseDto>
        {
            Success = true,
            Message = "Candidates details retrieved successfully.",
            Data = result,
            StatusCode = 200
        };
        return Ok(response);

    }
    [HttpGet("GetAll-Candidates")]
    [Authorize(Roles = "Admin,HR,Employee")]
    public async Task<IActionResult> GetAll([FromQuery] CareerFilterRequest request)
    {
        var result = await _careerService.GetAllAsync(request);

        var response = new ApiResponse<PagedResult<CareerCandidatedata>>
        {
            Success = true,
            Message = "Candidate details retrieved successfully.",
            Data = result,
            StatusCode = 200
        };
        return Ok(response);

    }
    [HttpGet("Download-Candidates")]
    [Authorize(Roles = "Admin,HR,Employee")]
    public async Task<IActionResult> DownloadCandidates(
    [FromQuery] CareerFilterRequest request)
    {
        var fileBytes = await _careerService
            .ExportCandidatesAsync(request);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Candidates_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
    [HttpDelete("Delete-Candidate-Profile/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var DeletedBy = GetCurrentUserId();
        await _careerService.DeleteAsync(id, DeletedBy);
        var response = new ApiResponse<Object>
        {

            Success = true,
            Message= "Candidate deleted Successfull",
            Data = null,
            Errors = null,
            StatusCode = 200
        };
        return Ok(response);
    }



}

