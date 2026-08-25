
    using EMS.Core.DTOs.Leave;
using EMS.Core.Enums;
    using EMS.Core.Helpers;
    using EMS.Data.Models;
    using EMS.Data.Repositories;
    using EMS.Data.UnitOfWork;
    using EMS.Service.Email;
using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.Extensions.Logging;

namespace EMS.Service.Leave;

    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IRepository<Employee> _employeeRepo;
        private readonly IRepository<Leavetype> _leaveTypeRepo;
        private readonly IRepository<Leaverequest> _leaveRequestRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogger<LeaveRequestService> _logger;

        public LeaveRequestService(
            IRepository<Employee> employeeRepo,
            IRepository<Leavetype> leaveTypeRepo,
            IRepository<Leaverequest> leaveRequestRepo,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<LeaveRequestService> logger)
        {
            _employeeRepo = employeeRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveRequestRepo = leaveRequestRepo;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

    // ---------- APPLY LEAVE ----------
    public async Task<LeaveRequestResponseDto> ApplyLeaveAsync(int employeeId, ApplyLeaveDto dto)
    {
        if (dto.EndDate < dto.StartDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        if (dto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Cannot apply leave for a past date.");

        var employee = await _employeeRepo.TableNoTracking
                       .FirstOrDefaultAsync(e => e.Id == employeeId && e.IsDeleted!=true);

        if(employee==null)  
              throw new KeyNotFoundException("Employee profile not found.");

        var leaveType= await _leaveTypeRepo.TableNoTracking
                              .FirstOrDefaultAsync(l => l.Id == dto.LeaveTypeId && l.IsDeleted!=true);

        if (leaveType==null)
            throw new KeyNotFoundException($"Leave Type {dto.LeaveTypeId} not found.");

        var hasOverlap = await _leaveRequestRepo.TableNoTracking
            .AnyAsync(l =>
                l.EmployeeId == employeeId &&
                !l.IsDeleted &&
                (l.Status == Status.Pending.ToString() || l.Status == Status.Approved.ToString()) &&
                dto.StartDate <= l.EndDate &&
                dto.EndDate >= l.StartDate);

        if (hasOverlap)
            throw new InvalidOperationException("Leave already exists for the selected dates.");

        var totalDays = (dto.EndDate.DayNumber - dto.StartDate.DayNumber) + 1;

        var leaveRequest = new Leaverequest
        {
            EmployeeId = employeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = Status.Pending.ToString(),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = employeeId
        };
        await _leaveRequestRepo.InsertAsync(leaveRequest);

        _logger.LogInformation(
            "Leave applied — EmployeeId: {EmployeeId}, LeaveTypeId: {LeaveTypeId}, StartDate: {StartDate}, EndDate: {EndDate}, TotalDays: {TotalDays}",
            employeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, totalDays);
        if (employee.ManagerId.HasValue)
        {
            var manager = await _employeeRepo.TableNoTracking
                .FirstOrDefaultAsync(e => e.Id == employee.ManagerId.Value);

            if (!string.IsNullOrEmpty(manager?.Email))
            {
                try
                {
                    var body = $@"
                    <h3>New Leave Request</h3>
                    <p><strong>{employee.Name}</strong> has applied for leave.</p>
                    <p>Leave Type: {leaveType.LeaveTypeName}</p>
                    <p>Dates: {dto.StartDate:dd-MMM-yyyy} to {dto.EndDate:dd-MMM-yyyy} ({totalDays} day(s))</p>
                    <p>Reason: {dto.Reason ?? "Not specified"}</p>";

                    await _emailService.SendEmailAsync(manager.Email, "New Leave Request - EMS", body);
                }
                catch (Exception emailEx)
                {
                    // Email failure should NOT roll back the leave request — log and continue
                    _logger.LogError(emailEx,
                        "Failed to send leave notification email — EmployeeId: {EmployeeId}, ManagerId: {ManagerId}",
                        employeeId, employee.ManagerId);
                }
            }
        }

        return new LeaveRequestResponseDto
        {
            Id = leaveRequest.Id,
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            LeaveTypeName = leaveType.LeaveTypeName,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            TotalDays = totalDays,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            CreatedDate = leaveRequest.CreatedDate
        };
    }


    public async Task<LeaveRequestResponseDto> GetLeavesAsync(int id)
    {

        var employee = await _employeeRepo.TableNoTracking
                       .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted != true);

        if (employee == null)
            throw new KeyNotFoundException("Employee Id not found");

        var leaveRequest = await _leaveRequestRepo.TableNoTracking
                            .Include(l => l.LeaveType)
                            .Include(l=>l.Employee)
                            .FirstOrDefaultAsync(l => l.EmployeeId == id && l.IsDeleted != true);

        var manager = await _employeeRepo.TableNoTracking
                            .Include(e=>e.Leaverequests)
                           
                            .FirstOrDefaultAsync(e => e.Id == employee.ManagerId && e.IsDeleted != true);
        return new LeaveRequestResponseDto
        {

            Id = employee.Id,
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            Email = employee.Email,
            LeaveTypeName = leaveRequest.LeaveType.LeaveTypeName,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            ApprovedByName = leaveRequest.ApprovedByEmployee != null
                    ? leaveRequest.ApprovedByEmployee.Name
                    : null,
            ActionDate = leaveRequest.ActionDate,
            RejectionReason=leaveRequest.RejectionReason,
            CreatedDate = leaveRequest.CreatedDate,

        };

    }
    // ---------- APPROVE ----------
    public async Task<LeaveRequestResponseDto> ApproveLeaveAsync(int leaveRequestId, int managerEmployeeId, UpdateleaveDto dto, int UpdatedBy)
    {
        var leave = await _leaveRequestRepo.Table
                       .Include(l => l.LeaveType)
                       .Include(l => l.Employee)
                       .FirstOrDefaultAsync(e => e.Id == leaveRequestId && e.IsDeleted != true);

        if (leave == null)
            throw new KeyNotFoundException("Employee not applied for leave.");

        var manager = await _employeeRepo.TableNoTracking
                       .FirstOrDefaultAsync(e => e.Id == managerEmployeeId && e.IsDeleted != true);

        if (leave.Employee.ManagerId != managerEmployeeId)
            throw new UnauthorizedAccessException("You are not authorized to approve this employee's leave.");

        if (leave.Status != Status.Pending.ToString())
            throw new InvalidOperationException($"This leave request has already been {leave.Status.ToLower()}.");

        if (!Enum.IsDefined(typeof(Status), dto.Status))
        {
            throw new ArgumentException(
                $"Invalid gender: {dto.Status}.");
        }

        //manager approve or reject leave
        leave.Status = dto.Status.ToString();
        leave.ApprovedBy = managerEmployeeId;
        leave.ActionDate = DateTime.UtcNow;
        leave.RejectionReason = dto.RejectionReason;
        //meta data
        leave.ModifiedDate = DateTime.UtcNow;
        leave.ModifiedBy = UpdatedBy;
        await _leaveRequestRepo.UpdateAsync(leave);

        _logger.LogInformation(
           "Leave approved — LeaveRequestId: {Id}, EmployeeId: {EmployeeId}, ApprovedBy: {ManagerId}",
           leaveRequestId, leave.EmployeeId, managerEmployeeId);


        if (!string.IsNullOrEmpty(leave.Employee.Email))
        {
            var body = $@"
                <h3>Leave Request {leave.Status}</h3>
                <p>Your leave request from <strong>{leave.StartDate:dd-MMM-yyyy}</strong> 
                   to <strong>{leave.EndDate:dd-MMM-yyyy}</strong> has been approved.</p>";

            await _emailService.SendEmailAsync(leave.Employee.Email, "Leave Request {leave.Status}  - EMS", body);
        }

        return new LeaveRequestResponseDto
        {


            Id = leave.Employee.Id,
            EmployeeId = leave.Employee.Id,
            EmployeeName = leave.Employee.Name,
            Email = leave.Employee.Email,
            LeaveTypeName = leave.LeaveType.LeaveTypeName,
            Reason = leave.Reason,
            Status = leave.Status,

            ApprovedByName = manager?.Name,
            ActionDate = leave.ActionDate,
            RejectionReason = leave .RejectionReason,

        };

    }

    public async Task<PagedResult<LeaveRequestResponseDto>> GetAllAsync(
    LeaveFilterRequestDto request,
    int managerEmployeeId)
    {
        var query = _leaveRequestRepo.TableNoTracking
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .Include(l => l.ApprovedByEmployee)
            .Where(l =>
                !l.IsDeleted &&
                l.Employee.ManagerId == managerEmployeeId);

        // Status filter
        if (request.Status.HasValue)
        {
            var status = request.Status.Value.ToString();

            query = query.Where(l => l.Status == status);
        }

        var resultQuery = query
            .Select(l => new LeaveRequestResponseDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.Name,
                Email = l.Employee.Email,
                LeaveTypeName = l.LeaveType.LeaveTypeName,

                StartDate = l.StartDate,
                EndDate = l.EndDate,

                TotalDays =
                    (l.EndDate.DayNumber - l.StartDate.DayNumber) + 1,

                Reason = l.Reason,
                Status = l.Status,

                ApprovedByName = l.ApprovedByEmployee != null
                    ? l.ApprovedByEmployee.Name
                    : null,

                ActionDate = l.ActionDate,
                RejectionReason = l.RejectionReason,
                CreatedDate = l.CreatedDate
            });

        return await resultQuery.ToPagedResultAsync(request);
    }



}

