using EMS.Core.DTOs.Designation;
using EMS.Core.DTOs.Documents;
using EMS.Core.DTOs.Employees;
using EMS.Core.Helpers;
using EMS.Data;
using EMS.Data.Models;
using EMS.Data.Repositories;
using EMS.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
namespace EMS.Service.Employees;


public class EmployeeService : IEmployeeService

{

    private readonly EmsDbContext _context;
    private readonly IRepository<User> _userRepo;
    private readonly IRepository<Employee> _employeeRepo;
    private readonly IRepository<Employeepersonaldetail> _personalDetailRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUrlHelperService _urlHelper;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        EmsDbContext context,
        IRepository<User> userRepo,
        IRepository<Employee> employeeRepo,
        IRepository<Employeepersonaldetail> personalDetailRepo,
        IUnitOfWork unitOfWork,
        IUrlHelperService urlHelper,
        ILogger<EmployeeService> logger)
    {
        _context = context;
        _userRepo = userRepo;
        _employeeRepo = employeeRepo;
        _personalDetailRepo = personalDetailRepo;
        _unitOfWork = unitOfWork;
        _urlHelper = urlHelper;
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

        var managerExists = await _employeeRepo.TableNoTracking
        .AnyAsync(e => e.Id == dto.ManagerId.Value && e.IsDeleted != true);

        if (!managerExists)
            throw new KeyNotFoundException($"Manager with Id {dto.ManagerId} not found.");

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
                Gender = dto.Gender,
                Dob = dto.DOB,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                DateOfJoining = dto.DateOfJoining,
                BloodGroup = dto.BloodGroup,
                DepartmentId = dto.DepartmentId,
                DesignationId = dto.DesignationId,
                RoleId = dto.RoleId,
                ManagerId = dto.ManagerId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _employeeRepo.AddAsync(employee);

            bool hasPersonalDetails = false;

            if (dto.PersonalDetails != null)
            {
                var pd = dto.PersonalDetails;

                var personalDetail = new Employeepersonaldetail
                {
                    Employee = employee,
                    MaritalStatus = pd.MaritalStatus,
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
                hasPersonalDetails = true;
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
                RoleName = role.RoleName,                          // ← already fetched, no extra query
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
            RoleName= employee.Role?.RoleName
          
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


    public async Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(PaginationRequest request)
    {
        var query = _employeeRepo.TableNoTracking
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.Id)
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                EmpNo = e.EmpNo,
                Name = e.Name,
                Email = e.Email,
                DepartmentName = e.Department != null ? e.Department.DepartmentName : null,
                DesignationName = e.Designation != null ? e.Designation.DesignationName : null,
                RoleName = e.Role != null ? e.Role.RoleName : null,
                DateOfJoining = e.DateOfJoining
            });

        return await query.ToPagedResultAsync(request);
    }



}






