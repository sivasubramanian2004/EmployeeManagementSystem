using EMS.Core.DTOs.JobPostings;
using EMS.Service.JobPostings;
using EMS.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace EMS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class JobPostingController : BaseController
    {

        private readonly IJobPostingService _jobPostingService;
        public JobPostingController(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        [Authorize(Roles = "HR, Manager,Employee")]
        [HttpPost("Create-JobPostings")]
        public async Task<IActionResult> Create([FromBody] CreateJobPostingDto dto) {

            var createdBy = GetCurrentUserId();
            var result = await _jobPostingService.CreateJobPostingAsync(dto, createdBy);
            var response = new ApiResponse<JobPostingsReponseDto>
            {
                Success = true,
                Message = "Job Posting created successfully.",
                Data = result,
                StatusCode = 201
            };

            return StatusCode(201, response);

        }

        [Authorize(Roles = "HR, Manager,Employee")]
        [HttpDelete("Delete-JobPosting/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedBy = GetCurrentUserId();
            await _jobPostingService.DeleteJobPostingAsync(id, deletedBy);
            var response = new ApiResponse<object>
            {
                Success = true,
                Message = "Job Posting deleted successfully.",
                Data = null,
                StatusCode = 200
            };
            return Ok(response);
        }

        [Authorize(Roles = "HR, Manager,Employee")]
        [HttpGet("Get-JobPosting/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _jobPostingService.GetJobPostingByIdAsync(id);
            var response = new ApiResponse<JobPostingsDetailsDto>
            {
                Success = true,
                Message = "Job Posting retrieved successfully.",
                Data = result,
                StatusCode = 200
            };
            return Ok(response);
        }


        [Authorize(Roles = "HR, Manager,Employee")]
        [HttpGet("GetAll-JobPosting")]
        public async Task<IActionResult> GetAll([FromQuery] JobPostingFilterRequest request)
        {
            var result = await _jobPostingService.GetAllJobPostingsAsync(request);
            var response = new ApiResponse<PagedResult<JobPostingsfilterDto>>
            {
                Success = true,
                Message = "Job Postings retrieved successfully.",
                Data = result,
                StatusCode = 200
            };
            return Ok(response);
        }

        [Authorize(Roles ="Employee, Manager,HR")]
        [HttpPut("Update-JobPosting")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobPostingDto dto)
        {
            var updatedBy = GetCurrentUserId();
            await _jobPostingService.UpdateJobPostingAsync(id, dto, updatedBy);
            var response = new ApiResponse<JobPostingsReponseDto>
            {
                Success = true,
                Message = "Job Posting updated successfully.",
                Data = null,
                StatusCode = 200
            };
            return Ok(response);
        }

    }
}
