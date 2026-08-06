using EMS.Core.DTOs.Documents;
using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Data.Repositories;
using EMS.Data.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EMS.Service.Documents;

public class EmployeeDocumentService : IEmployeeDocumentService
{
    private readonly IRepository<Employee> _employeeRepo;
    private readonly IRepository<Employeedocument> _documentRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FileStorageSettings _fileSettings;
    private readonly ILogger<EmployeeDocumentService> _logger;
    private readonly IUrlHelperService _urlHelper;
    private readonly string _webRootPath;

    public EmployeeDocumentService(
        IRepository<Employee> employeeRepo,
        IRepository<Employeedocument> documentRepo,
        IUnitOfWork unitOfWork,
        IOptions<FileStorageSettings> fileSettings,
        ILogger<EmployeeDocumentService> logger,
        IUrlHelperService urlHelper,
        IWebHostEnvironment env)
    {
        _employeeRepo = employeeRepo;
        _documentRepo = documentRepo;
        _unitOfWork = unitOfWork;
        _fileSettings = fileSettings.Value;
        _logger = logger;
        _urlHelper = urlHelper;
        // wwwroot path — if it doesn't exist yet (fresh project), WebRootPath can be null, so fallback
        _webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
    }

    public async Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentDto dto, int uploadedBy)
    {
        // ---------- Validation ----------

        if (dto.File == null || dto.File.Length == 0)
            throw new ArgumentException("No file was uploaded.");

        var maxSizeBytes = _fileSettings.MaxFileSizeInMB * 1024 * 1024;
        if (dto.File.Length > maxSizeBytes)
            throw new ArgumentException($"File size exceeds the maximum allowed limit of {_fileSettings.MaxFileSizeInMB} MB.");

        var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
        if (!_fileSettings.AllowedExtensions.Contains(extension))
            throw new ArgumentException(
                $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _fileSettings.AllowedExtensions)}");

        var employeeExists = await _employeeRepo.TableNoTracking
            .AnyAsync(e => e.Id == dto.EmployeeId && e.IsDeleted != true);

        if (!employeeExists)
            throw new KeyNotFoundException($"Employee with Id {dto.EmployeeId} not found.");

        // ---------- Save physical file ----------

        var uploadFolder = Path.Combine(_webRootPath, _fileSettings.BasePath, dto.EmployeeId.ToString());
        Directory.CreateDirectory(uploadFolder);   // creates wwwroot/UploadedDocuments/{employeeId}/ if not exists

        var uniqueFileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
        var fullPath = Path.Combine(uploadFolder, uniqueFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        // Relative path stored in DB — this becomes the public URL path too
        var relativePath = $"/{_fileSettings.BasePath}/{dto.EmployeeId}/{uniqueFileName}";

        var document = new Employeedocument
        {
            EmployeeId = dto.EmployeeId,
            DocumentType = dto.DocumentType,
            DocumentName = dto.File.FileName,
            FilePath = relativePath,   // e.g., "/UploadedDocuments/2/guid_resume.pdf"
            FileSize = (int)dto.File.Length,
            UploadedDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = uploadedBy
        };

        await _documentRepo.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Document uploaded — EmployeeId: {EmployeeId}, Type: {DocumentType}, File: {FileName}",
            dto.EmployeeId, dto.DocumentType, dto.File.FileName);

        return new DocumentResponseDto
        {
            Id = document.Id,
            EmployeeId = document.EmployeeId,
            DocumentType = document.DocumentType,
            DocumentName = document.DocumentName,
            FileSize = document.FileSize ?? 0,
            UploadedDate = document.UploadedDate,
            FileUrl = _urlHelper.BuildFullUrl(relativePath)
        };
    }
}
