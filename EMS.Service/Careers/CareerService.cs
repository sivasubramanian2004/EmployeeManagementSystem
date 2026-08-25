
using EMS.Core.DTOs.Careers;
using EMS.Core.DTOs.Documents;
using EMS.Core.DTOs.Employees;
using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Data.Repositories;
using EMS.Data.UnitOfWork;
using EMS.Service.Email;
using EMS.Service.FileStorage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using ClosedXML.Excel;

namespace EMS.Service.Careers
{   public class CareerService : ICareerService
    {
        private readonly IRepository<Career> _careerRepo;
        private readonly IRepository<Careereducation> _educationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogger<CareerService> _logger;
        private readonly IUrlHelperService _urlHelper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IRepository<Jobposting> _jobRepo;
        public CareerService(
            IRepository<Career> careerRepo,
             IRepository<Careereducation> educationRepo,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<CareerService> logger,
            IUrlHelperService urlHelper,
            IFileStorageService fileStorageService,
            IRepository<Jobposting> jobRepo )
        {
            _careerRepo = careerRepo;
            _educationRepo = educationRepo;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
            _urlHelper = urlHelper;
            _fileStorageService = fileStorageService;
            _jobRepo = jobRepo;
        }

        public async Task<CareerResponseDto> CreateAsync(CreateCareerDto dto)
        {
            var emailExists = await _careerRepo.Table
                              .Include(c=>c.JobPosting)
                              .AnyAsync(x => x.Email == dto.Email && x.JobPostingId == dto.JobPostingId && !x.IsDeleted);
            if (emailExists)
                throw new InvalidOperationException($"A career application  already exists with this email address for the role {dto.JobPostingId}.");

            var jobType=await _jobRepo.TableNoTracking.FirstOrDefaultAsync(j=>j.Id==dto.JobPostingId && !j.IsDeleted);

            if(jobType==null)    
                   throw new KeyNotFoundException($"Job posting with ID {dto.JobPostingId} not found.");

            if (dto.Educations == null || dto.Educations.Count == 0)
                throw new ArgumentException("At least one education detail is required.");

            // Validate education dates
            foreach (var education in dto.Educations)
            {
                if (education.ToDate.HasValue &&
                    education.ToDate.Value < education.FromDate)
                {
                    throw new ArgumentException(
                        $"Education ToDate cannot be earlier than FromDate for {education.Degree}.");
                }
            }
            FileUploadResult? resumePath=null;
            FileUploadResult? photoPath = null;
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var photofolder = Path.Combine("CareerDocuments", "CandidatePhotos");
                var resumefolder = Path.Combine("CareerDocuments", "CandidateResumes");

                if (dto.Photo != null)
                {
                    photoPath = await _fileStorageService.UploadAsync(dto.Photo, photofolder);
                }
                resumePath = await _fileStorageService.UploadAsync(dto.Resume, resumefolder);

                var career = new Career
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Mobile = dto.Mobile,
                    Street = dto.Street,
                    City = dto.City,
                    Pincode = dto.Pincode,
                    State = dto.State,
                    Country = dto.Country,
                    JobPostingId = dto.JobPostingId,
                    ReferralEmail = dto.ReferralEmail,
                    Experience = dto.Experience,
                    CurrentDesignation = dto.CurrentDesignation,
                    CurrentCompany = dto.CurrentCompany,
                    CurrentSalary = dto.CurrentSalary,
                    ExpectedSalary = dto.ExpectedSalary,
                    NoticePeriod = dto.NoticePeriod,
                    SkillSet = dto.SkillSet,
                    LinkedInUrl = dto.LinkedInUrl,
                    GitHubUrl = dto.GitHubUrl,
                    PhotoPath = photoPath?.RelativePath,
                    ResumePath = resumePath.RelativePath,
                    CreatedDate = DateTime.UtcNow
                };

                await _careerRepo.AddAsync(career);
                foreach (var educationDto in dto.Educations)
                {
                    career.Careereducations.Add(new Careereducation
                    {
                        EducationType = educationDto.EducationType.Trim(),
                        InstitutionName = educationDto.InstitutionName.Trim(),
                        Degree = educationDto.Degree.Trim(),
                        Percentage = educationDto.Percentage,
                        FromDate = educationDto.FromDate,
                        ToDate = educationDto.ToDate,
                        
                    });
                }
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();


                _logger.LogInformation(
                "Career application submitted by {Email}",
                career.Email);

                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Application Received</title>
</head>

<body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;'>

    <table width='100%' cellpadding='0' cellspacing='0' 
           style='background-color:#f4f6f8; padding:30px 15px;'>
        <tr>
            <td align='center'>

                <table width='600' cellpadding='0' cellspacing='0'
                       style='max-width:600px; width:100%; background:#ffffff;
                              border-radius:10px; overflow:hidden;
                              box-shadow:0 2px 8px rgba(0,0,0,0.08);'>

                    <!-- Header -->
                    <tr>
                        <td style='background:#198754; padding:25px 30px; text-align:center;'>
                            <h1 style='margin:0; color:#ffffff; font-size:24px;'>
                                Application Received
                            </h1>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style='padding:35px 30px; color:#333333;'>

                            <p style='font-size:16px; margin-top:0;'>
                                Dear <strong>{dto.FirstName}</strong>,
                            </p>

                            <p style='font-size:15px; line-height:1.7;'>
                                Thank you for your interest in joining our team.
                                We are pleased to inform you that your application
                                for the position of
                                <strong>{jobType.Title}</strong>
                                has been successfully received.
                            </p>

                            <div style='background:#f8f9fa; border-left:4px solid #198754;
                                        padding:15px 18px; margin:25px 0;'>
                                <p style='margin:0 0 8px; font-size:14px;'>
                                    <strong>Application Details</strong>
                                </p>

                                <p style='margin:5px 0; font-size:14px;'>
                                    Position:
                                    <strong>{jobType.Title}</strong>
                                </p>

                                <p style='margin:5px 0; font-size:14px;'>
                                    Applicant:
                                    <strong>{dto.FirstName} {dto.LastName}</strong>
                                </p>

                                <p style='margin:5px 0; font-size:14px;'>
                                    Email:
                                    <strong>{dto.Email}</strong>
                                </p>
                            </div>

                            <p style='font-size:15px; line-height:1.7;'>
                                Our recruitment team will carefully review your
                                application and qualifications. If your profile
                                matches our current requirements, we will contact
                                you regarding the next steps in the selection
                                process.
                            </p>

                            <p style='font-size:15px; line-height:1.7;'>
                                We appreciate the time and effort you have taken
                                to apply and thank you for considering us as a
                                potential employer.
                            </p>

                            <p style='font-size:15px; margin-top:30px;'>
                                Best regards,<br>
                                <strong>HR & Recruitment Team</strong><br>
                                EMS
                            </p>

                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background:#f8f9fa; padding:18px 30px;
                                   text-align:center; border-top:1px solid #eeeeee;'>

                            <p style='margin:0; font-size:12px; color:#777777;'>
                                This is an automated email. Please do not reply
                                to this email.
                            </p>

                            <p style='margin:8px 0 0; font-size:12px; color:#999999;'>
                                © {DateTime.UtcNow.Year} EMS. All rights reserved.
                            </p>

                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";
                await _emailService.SendEmailAsync(dto.Email, "Application Received - EMS", emailBody);

                return new CareerResponseDto
                {
                    Id = career.Id,
                    Email = career.Email,
                    Mobile = career.Mobile,
                    JobPostingPosition = jobType.Title,
                    PhotoUrl = _urlHelper.BuildFullUrl(career.PhotoPath),
                    ResumeUrl = _urlHelper.BuildFullUrl(career.ResumePath)
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                if (photoPath != null)
                    await _fileStorageService.DeleteAsync(photoPath.FullPath);

                if (resumePath != null)
                    await _fileStorageService.DeleteAsync(resumePath.FullPath);

                _logger.LogError(ex,
                    "Failed to create career application for Email: {Email}, JobApplicationPosition: {JobApplicationPosition}",
                    dto.Email, dto.JobPostingId);
                throw;
            }
        }
        public async Task<CareerAllResponseDto> GetCandidateDetailsByIdAsync(int id)
        {
            var candidate = await _careerRepo.TableNoTracking
                          .Include(c => c.Careereducations
                          .Where(e => !e.IsDeleted))
                          .Include(c => c.JobPosting)
                          .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (candidate == null)
                throw new KeyNotFoundException($"Candidate with ID {id} not found.");

            return new CareerAllResponseDto
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Email = candidate.Email,
                Mobile = candidate.Mobile,
                Street = candidate.Street,
                City = candidate.City,
                Pincode = candidate.Pincode,
                State = candidate.State,
                Country = candidate.Country,
                JobApplicationPosition = candidate.JobPosting.Title,
                Experience = candidate.Experience,
                CurrentDesignation = candidate.CurrentDesignation,
                CurrentCompany = candidate.CurrentCompany,
                CurrentSalary = candidate.CurrentSalary,
                ExpectedSalary = candidate.ExpectedSalary,
                NoticePeriod = candidate.NoticePeriod,
                SkillSet = candidate.SkillSet,
                LinkedInUrl = candidate.LinkedInUrl,
                GitHubUrl = candidate.GitHubUrl,
                PhotoUrl = string.IsNullOrWhiteSpace(candidate.PhotoPath)
                    ? null
                    : _urlHelper.BuildFullUrl(candidate.PhotoPath),
                ResumeUrl = string.IsNullOrWhiteSpace(candidate.ResumePath)
                    ? null
                    : _urlHelper.BuildFullUrl(candidate.ResumePath),
                Educations = candidate.Careereducations
                    .Where(e => !e.IsDeleted)
                    .Select(e => new CareerEducationResponseDto
                    {
                        Id = e.Id,
                        EducationType = e.EducationType,
                        InstitutionName = e.InstitutionName,
                        Degree = e.Degree,
                        Percentage = e.Percentage,
                        FromDate = e.FromDate,
                        ToDate = e.ToDate
                    })
                    .ToList()
            };
        }

        public async Task DeleteAsync(int id, int deletedBy)
        {
            var candidate = await _careerRepo.Table
                           .Include(c => c.Careereducations)
                          //  .Include(c => c.Careerdocuments)         // ← ADD THIS — include the new child collection
                           .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (candidate == null)
                throw new KeyNotFoundException($"Candidate Id {id} not found.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var deletedDate = DateTime.UtcNow;

                // Soft delete Career (parent)
                candidate.IsDeleted = true;
                candidate.DeletedDate = deletedDate;
                candidate.DeletedBy = deletedBy;

                // Soft delete Education records (child #1)
                var educationsToDelete = candidate.Careereducations
                    .Where(e => !e.IsDeleted)
                    .ToList();

                foreach (var education in educationsToDelete)
                {
                    education.IsDeleted = true;
                    education.DeletedDate = deletedDate;
                    education.DeletedBy = deletedBy;
                }              

                await _unitOfWork.SaveChangesAsync();   // ← Parent + BOTH children — still ONE round trip
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Candidate soft-deleted — CandidateId: {Id}, EducationRecordsDeleted: {EduCount}, , DeletedBy: {DeletedBy}",
                    id, educationsToDelete.Count, deletedBy);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex,
                    "Failed to soft-delete candidate — CandidateId: {Id}, DeletedBy: {DeletedBy}",
                    id, deletedBy);
                throw;
            }
        }


        public async Task<PagedResult<CareerCandidatedata>> GetAllAsync(
        CareerFilterRequest request)
        {

            var query = BuildCandidateQuery(request);

            // Career sorting options
            var sortOptions =
                new Dictionary<string, Expression<Func<Career, object>>>
                {
                    ["name"] = c => c.FirstName,
                    ["email"] = c => c.Email,
                    ["city"] = c => c.City,
                    ["position"] = c => c.JobPostingId,
                    ["experience"] = c => c.Experience,
                    ["applieddate"] = c => c.CreatedDate
                };

            // Generic sorting
            query = query.ApplySorting(request, sortOptions, defaultSort: c => c.Id);
            
            // Projection
            var resultQuery = query.Select(c => new CareerCandidatedata
            {
                Id = c.Id,
                Name = c.FirstName + " " + c.LastName,
                Email = c.Email,
                City = c.City,
                Mobile = c.Mobile,
                JobApplicationPosition = c.JobPosting.Title,
                Experience = c.Experience,
                ExpectedSalary = c.ExpectedSalary,
                AppliedDate = c.CreatedDate.Date
            });

            // Generic pagination
            return await resultQuery.ToPagedResultAsync(request);
        }

        public async Task<byte[]> ExportCandidatesAsync(
    CareerFilterRequest request)
        {
            var query = BuildCandidateQuery(request);

            var candidates = await query
                .OrderBy(c => c.Id)
                .Select(c => new CareerCandidatedata
                {
                    Id = c.Id,

                    Name = c.FirstName + " " + c.LastName,

                    Email = c.Email,

                    City = c.City,

                    Mobile = c.Mobile,

                    JobApplicationPosition = c.JobPosting.Title,

                    Experience = c.Experience,

                    ExpectedSalary = c.ExpectedSalary,

                    AppliedDate = c.CreatedDate.Date
                })
                .ToListAsync();

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Candidates");

            // Headers
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "City";
            worksheet.Cell(1, 5).Value = "Mobile";
            worksheet.Cell(1, 6).Value = "Position";
            worksheet.Cell(1, 7).Value = "Experience";
            worksheet.Cell(1, 8).Value = "Expected Salary";
            worksheet.Cell(1, 9).Value = "Applied Date";

            // Data
            for (int i = 0; i < candidates.Count; i++)
            {
                var row = i + 2;
                var candidate = candidates[i];

                worksheet.Cell(row, 1).Value = candidate.Id;
                worksheet.Cell(row, 2).Value = candidate.Name;
                worksheet.Cell(row, 3).Value = candidate.Email;
                worksheet.Cell(row, 4).Value = candidate.City;
                worksheet.Cell(row, 5).Value = candidate.Mobile;
                worksheet.Cell(row, 6).Value =
                    candidate.JobApplicationPosition;
                worksheet.Cell(row, 7).Value = candidate.Experience;
                worksheet.Cell(row, 8).Value = candidate.ExpectedSalary;
                worksheet.Cell(row, 9).Value = candidate.AppliedDate;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }
        private IQueryable<Career> BuildCandidateQuery(CareerFilterRequest request)
        {
            var query = _careerRepo.TableNoTracking
                .Where(c => !c.IsDeleted);

            // Name
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();

                query = query.Where(c =>
                    (c.FirstName + " " + c.LastName)
                    .Contains(name));
            }

            // Email
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var email = request.Email.Trim();

                query = query.Where(c =>
                    c.Email.Contains(email));
            }

            // City
            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var city = request.City.Trim();

                query = query.Where(c =>
                    c.City.Contains(city));
            }

            // Job Posting
            if (request.JobPostingId.HasValue)
            {
                query = query.Where(c =>
                    c.JobPostingId == request.JobPostingId.Value);
            }

            // Minimum Experience
            if (request.MinExperience.HasValue)
            {
                query = query.Where(c =>
                    c.Experience >= request.MinExperience.Value);
            }

            // Maximum Experience
            if (request.MaxExperience.HasValue)
            {
                query = query.Where(c =>
                    c.Experience <= request.MaxExperience.Value);
            }

            // Skill Set
            if (!string.IsNullOrWhiteSpace(request.SkillSet))
            {
                var skill = request.SkillSet.Trim();

                query = query.Where(c =>
                    c.SkillSet.Contains(skill));
            }

            // Applied Date From
            if (request.FromAppliedDate.HasValue)
            {
                var fromDate = request.FromAppliedDate.Value
                    .ToDateTime(TimeOnly.MinValue);

                query = query.Where(c =>
                    c.CreatedDate >= fromDate);
            }

            // Applied Date To
            if (request.ToAppliedDate.HasValue)
            {
                var toDate = request.ToAppliedDate.Value
                    .AddDays(1)
                    .ToDateTime(TimeOnly.MinValue);

                query = query.Where(c =>
                    c.CreatedDate < toDate);
            }

            return query;
        }

    }
}
