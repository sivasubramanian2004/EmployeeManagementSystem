using DocumentFormat.OpenXml.Wordprocessing;
using EMS.Core.DTOs.Designation;
using EMS.Core.DTOs.Documents;
using EMS.Core.DTOs.Employees;
using EMS.Core.Enums;
using EMS.Core.Helpers;
using EMS.Data;
using EMS.Data.Models;
using EMS.Data.Repositories;
using EMS.Data.UnitOfWork;
using EMS.Service.FileStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq.Expressions;
namespace EMS.Service.Employees;
public class EmployeeService : IEmployeeService

{

    private readonly EmsDbContext _context;
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Employee> _employeeRepo;
    private readonly IRepository<Employeepersonaldetail> _personalDetailRepo;
    private readonly IRepository<Employeedocument> _documentRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUrlHelperService _urlHelper;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IFileStorageService _fileStorageService;

    public EmployeeService(
        EmsDbContext context,
        IRepository<User> userRepo,
        IRepository<Employee> employeeRepo,
        IRepository<Employeepersonaldetail> personalDetailRepo,
        IRepository<Employeedocument> documentRepo,
        IUnitOfWork unitOfWork,
        IUrlHelperService urlHelper,
        ILogger<EmployeeService> logger,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _userRepo = userRepo;
        _employeeRepo = employeeRepo;
        _personalDetailRepo = personalDetailRepo;
        _documentRepo = documentRepo;
        _unitOfWork = unitOfWork;
        _urlHelper = urlHelper;
        _fileStorageService= fileStorageService;
       _logger = logger;
    }

    public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto, int createdBy)
    {
        // ---------- Pre-validation checks (before opening a transaction) ----------

        var user = await _userRepo.TableNoTracking
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsDeleted != true);

        if (user == null)
            throw new KeyNotFoundException(
                $"No registered account found for email '{dto.Email}'. The employee must register first.");

        var alreadyHasEmployeeProfile = await _employeeRepo.TableNoTracking
            .AnyAsync(e => e.UserId == user.Id && e.IsDeleted != true);

        if (alreadyHasEmployeeProfile)
            throw new InvalidOperationException($"An employee profile already exists for '{dto.Email}'.");

        var empNoExists = await _employeeRepo.TableNoTracking
            .AnyAsync(e => e.EmpNo == dto.EmpNo && e.IsDeleted != true);

        if (empNoExists)
            throw new InvalidOperationException($"Employee number '{dto.EmpNo}' is already in use.");

        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _employeeRepo.TableNoTracking
                .AnyAsync(e => e.Id == dto.ManagerId.Value && !e.IsDeleted);

            if (!managerExists)
            {
                throw new KeyNotFoundException($"Manager with Id {dto.ManagerId.Value} not found.");

            }
        }
        // Pre-validation — fetch WITH names, instead of just checking existence
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId && d.IsDeleted != true)
            ?? throw new KeyNotFoundException($"Department with Id {dto.DepartmentId} not found.");

        var designation = await _context.Designations
            .FirstOrDefaultAsync(d => d.Id == dto.DesignationId && d.IsDeleted != true)
            ?? throw new KeyNotFoundException($"Designation with Id {dto.DesignationId} not found.");

        if (dto.PersonalDetails?.MaritalStatus.HasValue == true &&
         !Enum.IsDefined(dto.PersonalDetails.MaritalStatus.Value))
        {
            throw new ArgumentException(
                $"Invalid marital status: {dto.PersonalDetails.MaritalStatus.Value}.");
        }
        if (dto.BloodGroup.HasValue && !Enum.IsDefined(typeof(BloodGroup), dto.BloodGroup))
        {
            throw new ArgumentException(
                $"Invalid blood group: {dto.BloodGroup}.");
        }

        if (!Enum.IsDefined(typeof(Gender), dto.Gender))
        {
            throw new ArgumentException(
                $"Invalid gender: {dto.Gender}.");
        }
            // ---------- Transaction: Employee + PersonalDetails together ----------

            await _unitOfWork.BeginTransactionAsync();
        try
        {
            var employee = new Employee
            {
                UserId = user.Id,
                EmpNo = dto.EmpNo,
                Name = dto.Name,
                Age = dto.Age,
                Gender = dto.Gender.ToString(),
                Dob = dto.DOB,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                DateOfJoining = dto.DateOfJoining,
                BloodGroup = dto.BloodGroup.ToString(),
                DepartmentId = dto.DepartmentId,
                DesignationId = dto.DesignationId,
              //  RoleId = dto.RoleId,
                ManagerId = dto.ManagerId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _employeeRepo.AddAsync(employee);


            if (dto.PersonalDetails != null)
            {
                var pd = dto.PersonalDetails;

                var personalDetail = new Employeepersonaldetail
                {
                    Employee = employee,
                    MaritalStatus = pd.MaritalStatus.ToString(),
                    Nationality = pd.Nationality,
                    AadharNumber = pd.AadharNumber,
                    PanNumber = pd.PanNumber,
                    EmergencyContactName = pd.EmergencyContactName,
                    EmergencyContactPhone = pd.EmergencyContactPhone,
                    EmergencyContactRelation = pd.EmergencyContactRelation,
                    BankAccountNumber = pd.BankAccountNumber,
                    BankName = pd.BankName,
                    IfscCode = pd.IfscCode,
                    District = pd.District,
                    State = pd.State,
                    Pincode = pd.Pincode,
                    FatherName = pd.FatherName,
                    FatherOccupation = pd.FatherOccupation,
                    FatherMobileNo = pd.FatherMobileNo,
                    MotherName = pd.MotherName,
                    MotherOccupation = pd.MotherOccupation,
                    SiblingCount = pd.SiblingCount,
                    FamilyIncome = pd.FamilyIncome,
                    IsFirstGraduate = pd.IsFirstGraduate,
                    SslcSchoolName = pd.SslcSchoolName,
                    SslcPercentage = pd.SslcPercentage,
                    HscOrDiplomaSchoolName = pd.HscOrDiplomaSchoolName,
                    HscOrDiplomaPercentage = pd.HscOrDiplomaPercentage,
                    UgCollegeName = pd.UgCollegeName,
                    UgCgpa = pd.UgCgpa,
                    PgDegreeCollegeName = pd.PgDegreeCollegeName,
                    PgCgpa = pd.PgCgpa,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                await _personalDetailRepo.AddAsync(personalDetail);
            }

            await _unitOfWork.SaveChangesAsync();          // ✅ ONE round trip
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation(
                "Employee profile created — EmpNo: {EmpNo}, Email: {Email}, CreatedBy: {CreatedBy}",
                dto.EmpNo, dto.Email, createdBy);

            return new EmployeeResponseDto
            {
                Id = employee.Id,                          // auto-filled after SaveChangesAsync()
                EmpNo = employee.EmpNo,
                Name = employee.Name,
                Email = employee.Email ?? string.Empty,
                DepartmentName = department.DepartmentName,     // ← already fetched, no extra query
                DesignationName = designation.DesignationName,   // ← already fetched, no extra query
                DateOfJoining = employee.DateOfJoining,
               
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex,
                "Failed to create employee profile for Email: {Email}, EmpNo: {EmpNo}",
                dto.Email, dto.EmpNo);
            throw;  
        }
    }
    public async Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentDto dto, int uploadedBy)
    {
        // ---------------- Validation ----------------
        if (!Enum.IsDefined(typeof(EMS.Core.Enums.DocumentType), dto.DocumentType))
        {
            throw new ArgumentException(
                $"Invalid document type: {dto.DocumentType}.");
        }
        var employeeExists = await _employeeRepo.TableNoTracking
                             .AnyAsync(e => e.Id == dto.EmployeeId && e.IsDeleted != true);

        if (!employeeExists)
        {
            throw new KeyNotFoundException($"Employee with Id {dto.EmployeeId} not found.");
        }

        // ---------------- Existing Document ----------------

        var existingDocument = await _documentRepo.Table
                               .FirstOrDefaultAsync(d =>
                               d.EmployeeId == dto.EmployeeId &&
                               d.DocumentType == dto.DocumentType.ToString() &&
                               d.IsDeleted != true);

        if (existingDocument != null)
        {
            existingDocument.IsDeleted = true;
            existingDocument.DeletedDate = DateTime.UtcNow;
            existingDocument.DeletedBy = uploadedBy;
        }

        // ---------------- Upload File ----------------

        var folder = Path.Combine("EmployeesDocuments", dto.DocumentType.ToString());

        FileUploadResult uploadResult;

        uploadResult = await _fileStorageService.UploadAsync(dto.File, folder);

        // ---------------- Create DB Record ----------------
        try
        {
            var document = new Employeedocument
            {
            EmployeeId = dto.EmployeeId,

            DocumentType = dto.DocumentType.ToString(),

            DocumentName = uploadResult.OriginalFileName,

            FilePath = uploadResult.RelativePath,

            FileSize = (int)uploadResult.FileSize,

            UploadedDate = DateTime.UtcNow,

            CreatedDate = DateTime.UtcNow,

            CreatedBy = uploadedBy
            };

        // ---------------- Save DB ----------------
            await _documentRepo.AddAsync(document);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
            "Document uploaded successfully. EmployeeId: {EmployeeId}, DocumentType: {DocumentType}, FileName: {FileName}",
            dto.EmployeeId,
            dto.DocumentType,
            uploadResult.OriginalFileName);
            return new DocumentResponseDto
            {
                Id = document.Id,
                DocumentType = document.DocumentType,
                DocumentName = document.DocumentName,
                FileSize = document.FileSize,
                UploadedDate = document.UploadedDate,
                FileUrl = _urlHelper.BuildFullUrl(document.FilePath),
            };

        }
        catch (Exception ex)
        {
            // DB failed → remove physical file
            await _fileStorageService.DeleteAsync(
                uploadResult.FullPath);

            _logger.LogError(
                ex,
                "Failed to save document record. EmployeeId: {EmployeeId}, DocumentType: {DocumentType}",
                dto.EmployeeId,
                dto.DocumentType);

            throw;
        }
    }
    public async Task<EmployeeFullDetailsDto?> GetEmployeeFullDetailsByIdAsync(int employeeId)
    {
        var employee = await _employeeRepo.TableNoTracking
                      .Include(e => e.Employeepersonaldetail)
                      .Include(e => e.Employeedocuments)
                      .Include(e => e.Department)
                      .Include(e => e.Designation)
                      .Include(e => e.Role)
                      .AsSplitQuery()
                      .FirstOrDefaultAsync(e => e.Id == employeeId && e.IsDeleted != true);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with Id {employeeId} not found.");

        var dto = new EmployeeFullDetailsDto
        {
            Id = employee.Id,
            Email = employee.Email,
            EmpNo = employee.EmpNo,
            Name = employee.Name,
            Age = employee.Age,
            Gender = employee.Gender,
            DOB = employee.Dob,
            Phone = employee.Phone,
            Address = employee.Address,
            DateOfJoining = employee.DateOfJoining,
            BloodGroup = employee.BloodGroup,
            DepartmentName= employee.Department?.DepartmentName,
            DesignationName= employee.Designation?.DesignationName,
         //   RoleName= employee.Role?.RoleName
          
        };

        // ---------- PersonalDetails (one-to-one) ----------
        if (employee.Employeepersonaldetail != null)
        {
            var p = employee.Employeepersonaldetail;

            dto.PersonalDetails = new PersonalDetailsResponseDto
            {
                Id= p.Id,
                EmployeeId = p.EmployeeId,
                MaritalStatus =p.MaritalStatus,
                Nationality = p.Nationality,
                AadharNumber =p.AadharNumber,      // ⚠️ unmasked — decide masking policy, see earlier note
                PanNumber = p.PanNumber,            // ⚠️ unmasked — decide masking policy, see earlier note
                EmergencyContactName = p.EmergencyContactName,
                EmergencyContactPhone = p.EmergencyContactPhone,
                EmergencyContactRelation = p.EmergencyContactRelation,
                BankAccountNumber = p.BankAccountNumber,   // ⚠️ unmasked
                BankName = p.BankName,
                IfscCode = p.IfscCode,
                District = p.District,
                State = p.State,
                Pincode = p.Pincode,
                FatherName = p.FatherName,
                FatherOccupation = p.FatherOccupation,
                FatherMobileNo = p.FatherMobileNo,
                MotherName = p.MotherName,
                MotherOccupation = p.MotherOccupation,
                SiblingCount = p.SiblingCount,
                FamilyIncome = p.FamilyIncome,
                IsFirstGraduate = p.IsFirstGraduate,
                SslcSchoolName = p.SslcSchoolName,
                SslcPercentage = p.SslcPercentage,
                HscOrDiplomaSchoolName = p.HscOrDiplomaSchoolName,
                HscOrDiplomaPercentage = p.HscOrDiplomaPercentage,
                UgCollegeName = p.UgCollegeName,
                UgCgpa = p.UgCgpa,
                PgDegreeCollegeName = p.PgDegreeCollegeName,
                PgCgpa = p.PgCgpa
            };
        }

        // ---------- Documents (one-to-many) ----------
        dto.Documents = employee.Employeedocuments
            .Where(d => d.IsDeleted != true)
            .Select(d => new DocumentResponseDto
            {
                Id=d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FileSize = d.FileSize ?? 0,
                UploadedDate = d.UploadedDate,
                FileUrl = _urlHelper.BuildFullUrl(d.FilePath)
            })
            .ToList();

        return dto;
    }


    public async Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(EmployeeFilterRequest request)
    {
        var query = _employeeRepo.TableNoTracking
            .Where(e => !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(e => e.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim();
            query = query.Where(e =>e.Email.Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(request.EmpNo))
        {
            var empNo = request.EmpNo.Trim();
            query = query.Where(e => e.EmpNo.Contains(empNo));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        if (request.DesignationId.HasValue)
        {
            query = query.Where(e => e.DesignationId == request.DesignationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.MaritalStatus))
        {
            var status = request.MaritalStatus.Trim();
            query = query.Where(e =>
                e.Employeepersonaldetail != null &&
                e.Employeepersonaldetail.MaritalStatus != null &&
                e.Employeepersonaldetail.MaritalStatus.Contains(status));
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phone = request.Phone.Trim();
            query = query.Where(e => e.Phone != null && e.Phone.Contains(phone));
        }

        var sortOptions = new Dictionary<string, Expression<Func<Employee, object>>>
        {
            ["name"] = e => e.Name,
            ["email"] = e => e.Email,
            ["dateofjoining"] = e => e.DateOfJoining,
            ["empno"] = e => e.EmpNo,
            ["age"] = e => e.Age
        };

        query = query.ApplySorting(request, sortOptions, defaultSort: e => e.Id);

        var resultQuery = query.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            EmpNo = e.EmpNo,
            Name = e.Name,
            Email = e.Email,
            DepartmentName = e.Department.DepartmentName,
            DesignationName=e.Designation.DesignationName,
            //RoleName =e.Role.RoleName,
            DateOfJoining = e.DateOfJoining
        });

        return await resultQuery.ToPagedResultAsync(request);
    }

    public async Task DeleteAsync(int id, int deletedBy)
    {

        var employee = await _employeeRepo.Table
                     .Include(e => e.Employeedocuments)
                     .Include(e => e.Employeepersonaldetail)
                     .Include(e => e.User)
                     .FirstOrDefaultAsync(e => e.IsDeleted != true && e.Id == id);

        if (employee == null)
            throw new KeyNotFoundException($"Profile not found Id {id} ");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deletedDate = DateTime.UtcNow;
            employee.IsDeleted = true;
            employee.IsActive = false;
            employee.DeletedDate = deletedDate;
            employee.DeletedBy = deletedBy;

            var documents = employee.Employeedocuments
                                 .Where(c => c.IsDeleted != true)
                                 .ToList();
            foreach (var item in documents)
            {
                item.IsDeleted = true;
                item.DeletedBy = deletedBy;
                item.DeletedDate = DateTime.UtcNow;
                item.DeletedBy = deletedBy;
            }
            if (employee.Employeepersonaldetail != null) { 
                 var personaldetails = employee.Employeepersonaldetail;
                 personaldetails.IsDeleted = true;
                 personaldetails.IsActive = false;
                 personaldetails.DeletedDate = deletedDate;
                 personaldetails.DeletedBy = deletedBy;
            }
            if (employee.User != null)
            {
                var user = employee.User;
                user.IsDeleted = true;
                user.IsActive = false;
                user.DeletedDate = deletedDate;
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Employee profile deleted — EmpNo: {EmpNo}, Email: {Email}, DeletedBy: {DeletedBy}",
                employee.EmpNo, employee.Email, deletedBy);

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "Failed to soft-delete employee — EmployeeId: {EmployeeId}, DeletedBy: {DeletedBy}", id, deletedBy);

            throw;
        }
    }


    public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto, int updatedBy) { 
    
    
     var employee = await _employeeRepo.Table
                   .Include(e => e.Employeepersonaldetail)
                   .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted != true);
     
    if(employee==null)
            throw new KeyNotFoundException($"Employee with Id {id} not found.");

        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _employeeRepo.TableNoTracking
                .AnyAsync(e => e.Id == dto.ManagerId.Value && !e.IsDeleted);

            if (!managerExists)
            {
                throw new KeyNotFoundException($"Manager with Id {dto.ManagerId.Value} not found.");

            }
        }
        // Pre-validation — fetch WITH names, instead of just checking existence
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == dto.DepartmentId && d.IsDeleted != true)
            ?? throw new KeyNotFoundException($"Department with Id {dto.DepartmentId} not found.");

        var designation = await _context.Designations
            .FirstOrDefaultAsync(d => d.Id == dto.DesignationId && d.IsDeleted != true)
            ?? throw new KeyNotFoundException($"Designation with Id {dto.DesignationId} not found.");

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.IsDeleted != true)
            ?? throw new KeyNotFoundException($"Role with Id {dto.RoleId} not found.");

        if (dto.PersonalDetails?.MaritalStatus.HasValue == true &&
         !Enum.IsDefined(dto.PersonalDetails.MaritalStatus.Value))
        {
            throw new ArgumentException(
                $"Invalid marital status: {dto.PersonalDetails.MaritalStatus.Value}.");
        }
        if (dto.BloodGroup.HasValue && !Enum.IsDefined(typeof(BloodGroup), dto.BloodGroup))
        {
            throw new ArgumentException(
                $"Invalid blood group: {dto.BloodGroup}.");
        }

        if (!Enum.IsDefined(typeof(Gender), dto.Gender))
        {
            throw new ArgumentException(
                $"Invalid gender: {dto.Gender}.");
        }
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            employee.Name = dto.Name;
            employee.Age = dto.Age;
            employee.Gender = dto.Gender.ToString();
            employee.Email = dto.Email;
            employee.Phone = dto.Phone;
            employee.Address = dto.Address;
            employee.Dob = dto.DOB;
            employee.DateOfJoining = dto.DateOfJoining;
            employee.BloodGroup = dto.BloodGroup?.ToString();
            employee.DepartmentId = dto.DepartmentId;
            employee.DesignationId = dto.DesignationId;
            employee.ManagerId = dto.ManagerId;
            employee.ModifiedDate = DateTime.UtcNow;
            employee.ModifiedBy = updatedBy;
            if (employee.Employeepersonaldetail != null && dto.PersonalDetails != null)
            {
                var personaldetails = employee.Employeepersonaldetail;
                personaldetails.MaritalStatus = dto.PersonalDetails.MaritalStatus?.ToString();
                personaldetails.Nationality = dto.PersonalDetails.Nationality;
                personaldetails.AadharNumber = dto.PersonalDetails.AadharNumber;
                personaldetails.PanNumber = dto.PersonalDetails.PanNumber;
                personaldetails.EmergencyContactName = dto.PersonalDetails.EmergencyContactName;
                personaldetails.EmergencyContactPhone = dto.PersonalDetails.EmergencyContactPhone;
                personaldetails.EmergencyContactRelation = dto.PersonalDetails.EmergencyContactRelation;
                personaldetails.BankAccountNumber = dto.PersonalDetails.BankAccountNumber;
                personaldetails.BankName = dto.PersonalDetails.BankName;
                personaldetails.IfscCode = dto.PersonalDetails.IfscCode;
                personaldetails.District = dto.PersonalDetails.District;
                personaldetails.State = dto.PersonalDetails.State;
                personaldetails.Pincode = dto.PersonalDetails.Pincode;
                personaldetails.FatherName = dto.PersonalDetails.FatherName;
                personaldetails.FatherOccupation = dto.PersonalDetails.FatherOccupation;
                personaldetails.FatherMobileNo = dto.PersonalDetails.FatherMobileNo;
                personaldetails.MotherName = dto.PersonalDetails.MotherName;
                personaldetails.MotherOccupation = dto.PersonalDetails.MotherOccupation;
                personaldetails.SiblingCount = dto.PersonalDetails.SiblingCount;
                personaldetails.FamilyIncome = dto.PersonalDetails.FamilyIncome;
                personaldetails.IsFirstGraduate = dto.PersonalDetails.IsFirstGraduate;
                personaldetails.SslcSchoolName = dto.PersonalDetails.SslcSchoolName;
                personaldetails.SslcPercentage = dto.PersonalDetails.SslcPercentage;
                personaldetails.HscOrDiplomaSchoolName = dto.PersonalDetails.HscOrDiplomaSchoolName;
                personaldetails.HscOrDiplomaPercentage = dto.PersonalDetails.HscOrDiplomaPercentage;
                personaldetails.UgCollegeName = dto.PersonalDetails.UgCollegeName;
                personaldetails.UgCgpa = dto.PersonalDetails.UgCgpa;
                personaldetails.PgDegreeCollegeName = dto.PersonalDetails.PgDegreeCollegeName;
                personaldetails.PgCgpa = dto.PersonalDetails.PgCgpa;
                personaldetails.ModifiedDate = DateTime.UtcNow;
                personaldetails.ModifiedBy = updatedBy;

            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            _logger.LogInformation("Employee profile updated — EmpNo: {EmpNo}, Email: {Email}, UpdatedBy: {UpdatedBy}",
                        employee.EmpNo, employee.Email, updatedBy);

            return new EmployeeResponseDto
            {

                Id = employee.Id,
                EmpNo = employee.EmpNo,
                Name = employee.Name,
                Email = employee.Email

            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to update employee profile — EmployeeId: {EmployeeId}, UpdatedBy: {UpdatedBy}", id, updatedBy);
            throw;

        }
    }
}






