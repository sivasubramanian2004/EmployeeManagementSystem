using EMS.Service.Documents;
using EMS.Core.DTOs.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Core.Helpers;

namespace EMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeDocumentController : BaseController
{
    private readonly IEmployeeDocumentService _service;

    public EmployeeDocumentController(IEmployeeDocumentService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    [Authorize]
    [RequestSizeLimit(10 * 1024 * 1024)]   // 10 MB hard cap at the request level (extra safety, beyond service-level check)
    public async Task<IActionResult> Upload([FromForm] UploadDocumentDto dto)
    {
        var uploadedBy = GetCurrentUserId();
        var result = await _service.UploadDocumentAsync(dto, uploadedBy);

        var response = new ApiResponse<DocumentResponseDto>
        {
            Success = true,
            Message = "Document uploaded successfully.",
            Data = result,
            StatusCode = 201
        };

        return StatusCode(201, response);
    }
}