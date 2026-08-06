
    using EMS.Core.Helpers;
    using EMS.Data.Models;
    using EMS.Core.DTOs.Leave;
    using global::EMS.Data.Repositories;
    using global::EMS.Data.UnitOfWork;
    using global::EMS.Service.Email;
    using Microsoft.EntityFrameworkCore;
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
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted)
            ?? throw new KeyNotFoundException("Employee profile not found.");

        var leaveTypeExists = await _leaveTypeRepo.TableNoTracking
            .AnyAsync(l => l.Id == dto.LeaveTypeId && !l.IsDeleted);

        if (!leaveTypeExists)
            throw new KeyNotFoundException($"Leave Type {dto.LeaveTypeId} not found.");

        var hasOverlap = await _leaveRequestRepo.TableNoTracking
            .AnyAsync(l =>
                l.EmployeeId == employeeId &&
                !l.IsDeleted &&
                (l.Status == "Pending" || l.Status == "Approved") &&
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
            Status = "Pending",
            CreatedDate = DateTime.UtcNow,
            CreatedBy = employeeId
        };

        await _leaveRequestRepo.AddAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        if (employee.ManagerId.HasValue)
        {
            var manager = await _employeeRepo.TableNoTracking
                .FirstOrDefaultAsync(x => x.Id == employee.ManagerId.Value);

            if (manager != null && !string.IsNullOrWhiteSpace(manager.Email))
            {
                var body = $@"
            <h3>New Leave Request</h3>
            <p><strong>{employee.Name}</strong> applied for leave.</p>
            <p><b>Dates:</b> {dto.StartDate:dd-MMM-yyyy} - {dto.EndDate:dd-MMM-yyyy}</p>
            <p><b>Total Days:</b> {totalDays}</p>
            <p><b>Reason:</b> {dto.Reason}</p>";

                await _emailService.SendEmailAsync(manager.Email, "New Leave Request", body);
            }
        }

        return await MapToResponseDto(leaveRequest.Id);
    }

    // ---------- GET MY LEAVES (employee's own requests) ----------
    public async Task<List<LeaveRequestResponseDto>> GetMyLeavesAsync(int employeeId)
    {
        return await _leaveRequestRepo.TableNoTracking
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .Include(x => x.ApprovedByNavigation)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new LeaveRequestResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.Name,
                LeaveTypeName = x.LeaveType.LeaveTypeName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TotalDays = x.EndDate.DayNumber - x.StartDate.DayNumber + 1,
                Reason = x.Reason,
                Status = x.Status,
                ApprovedByName = x.ApprovedByNavigation != null
                    ? x.ApprovedByNavigation.Name
                    : null,
                ActionDate = x.ActionDate,
                CreatedDate = x.CreatedDate,
                RejectionReason = x.RejectionReason,
                
            })
            .ToListAsync();
    }

    // ---------- GET PENDING APPROVALS (for a manager) ----------
    public async Task<List<LeaveRequestResponseDto>> GetPendingApprovalsAsync(int managerEmployeeId)
    {
        return await _leaveRequestRepo.TableNoTracking
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .Where(x => x.Employee.ManagerId == managerEmployeeId &&
                        x.Status == "Pending" &&
                        !x.IsDeleted)
            .OrderBy(x => x.StartDate)
            .Select(x => new LeaveRequestResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.Name,
                LeaveTypeName = x.LeaveType.LeaveTypeName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TotalDays = x.EndDate.DayNumber - x.StartDate.DayNumber + 1,
                Reason = x.Reason,
                Status = x.Status,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync();
    }
    // ---------- APPROVE ----------
    public async Task<LeaveRequestResponseDto> ApproveLeaveAsync(int leaveRequestId, int managerEmployeeId)
    {
        var leaveRequest = await _leaveRequestRepo.Table   // tracked — we're modifying it
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.Id == leaveRequestId && l.IsDeleted != true)
            ?? throw new KeyNotFoundException($"Leave request with Id {leaveRequestId} not found.");

        if (leaveRequest.Employee.ManagerId != managerEmployeeId)
            throw new UnauthorizedAccessException("You are not authorized to approve this employee's leave.");

        if (leaveRequest.Status != "Pending")
            throw new InvalidOperationException($"This leave request has already been {leaveRequest.Status.ToLower()}.");

        leaveRequest.Status = "Approved";
        leaveRequest.ApprovedBy = managerEmployeeId;
        leaveRequest.ActionDate = DateTime.UtcNow;
        leaveRequest.ModifiedDate = DateTime.UtcNow;
        leaveRequest.ModifiedBy = managerEmployeeId;

        _leaveRequestRepo.UpdateEntity(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Leave approved — LeaveRequestId: {Id}, EmployeeId: {EmployeeId}, ApprovedBy: {ManagerId}",
            leaveRequestId, leaveRequest.EmployeeId, managerEmployeeId);

        if (!string.IsNullOrEmpty(leaveRequest.Employee.Email))
        {
            var body = $@"
                <h3>Leave Request Approved</h3>
                <p>Your leave request from <strong>{leaveRequest.StartDate:dd-MMM-yyyy}</strong> 
                   to <strong>{leaveRequest.EndDate:dd-MMM-yyyy}</strong> has been approved.</p>";

            await _emailService.SendEmailAsync(leaveRequest.Employee.Email, "Leave Request Approved - EMS", body);
        }

        return await MapToResponseDto(leaveRequest.Id);
    }

    // ---------- REJECT ----------
    public async Task<LeaveRequestResponseDto> RejectLeaveAsync(int leaveRequestId, int managerEmployeeId, RejectLeaveDto dto)
        {
            var leaveRequest = await _leaveRequestRepo.Table
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == leaveRequestId && l.IsDeleted != true)
                ?? throw new KeyNotFoundException($"Leave request with Id {leaveRequestId} not found.");

            if (leaveRequest.Employee.ManagerId != managerEmployeeId)
                throw new UnauthorizedAccessException("You are not authorized to reject this employee's leave.");

            if (leaveRequest.Status != "Pending")
                throw new InvalidOperationException($"This leave request has already been {leaveRequest.Status.ToLower()}.");

            leaveRequest.Status = "Rejected";
            leaveRequest.ApprovedBy = managerEmployeeId;
            leaveRequest.ActionDate = DateTime.UtcNow;
            leaveRequest.RejectionReason = dto.RejectionReason;
            leaveRequest.ModifiedDate = DateTime.UtcNow;
            leaveRequest.ModifiedBy = managerEmployeeId;

            _leaveRequestRepo.UpdateEntity(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Leave rejected — LeaveRequestId: {Id}, EmployeeId: {EmployeeId}, RejectedBy: {ManagerId}",
                leaveRequestId, leaveRequest.EmployeeId, managerEmployeeId);

            if (!string.IsNullOrEmpty(leaveRequest.Employee.Email))
            {
                var body = $@"
                <h3>Leave Request Rejected</h3>
                <p>Your leave request from <strong>{leaveRequest.StartDate:dd-MMM-yyyy}</strong> 
                   to <strong>{leaveRequest.EndDate:dd-MMM-yyyy}</strong> has been rejected.</p>
                <p>Reason: {dto.RejectionReason}</p>";

                await _emailService.SendEmailAsync(leaveRequest.Employee.Email, "Leave Request Rejected - EMS", body);
            }

            return await MapToResponseDto(leaveRequest.Id);
        }

    // ---------- Shared mapping helper ----------
    private async Task<LeaveRequestResponseDto> MapToResponseDto(int leaveRequestId)
    {
        var leave = await _leaveRequestRepo.TableNoTracking
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .FirstAsync(x => x.Id == leaveRequestId);

        string? approvedByName = null;

        if (leave.ApprovedBy.HasValue)
        {
            approvedByName = await _employeeRepo.TableNoTracking
                .Where(e => e.Id == leave.ApprovedBy.Value)
                .Select(e => e.Name)
                .FirstOrDefaultAsync();
        }

        return new LeaveRequestResponseDto
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId,
            EmployeeName = leave.Employee.Name,
            LeaveTypeName = leave.LeaveType.LeaveTypeName,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            TotalDays = leave.EndDate.DayNumber - leave.StartDate.DayNumber + 1,
            Reason = leave.Reason,
            Status = leave.Status,
            ApprovedByName = approvedByName,
            ActionDate = leave.ActionDate,
            RejectionReason = leave.RejectionReason,
            CreatedDate = leave.CreatedDate
        };
    }
}

