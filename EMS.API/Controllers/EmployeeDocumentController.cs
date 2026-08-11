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

    
}