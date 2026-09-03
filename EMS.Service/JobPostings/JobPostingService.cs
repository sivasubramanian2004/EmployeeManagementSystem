using EMS.Core.DTOs.Careers;
using EMS.Core.DTOs.JobPostings;
using EMS.Core.Enums;
using EMS.Core.Helpers;
using EMS.Data.Models;
using EMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
namespace EMS.Service.JobPostings
{
    public class JobPostingService : IJobPostingService
    {
       
        private readonly IRepository<Jobposting> _jobPostingRepo;
        private readonly ILogger<JobPostingService> _logger;
        public JobPostingService(IRepository<Jobposting> jobPostingRepo, ILogger<JobPostingService> logger)
        {
             _jobPostingRepo= jobPostingRepo;
             _logger = logger;

        }

        public async Task<JobPostingsReponseDto> CreateJobPostingAsync(CreateJobPostingDto dto, int createdBy) {

            if (!Enum.IsDefined(typeof(EmploymentType), dto.EmploymentType))
            {
                throw new ArgumentException($"Invalid document type: {dto.EmploymentType}.");
            }
            if (!Enum.IsDefined(typeof(WorkMode), dto.WorkMode))
            {
                throw new ArgumentException($"Invalid document type: {dto.WorkMode}.");
            }

            // In JobPostingService.CreateAsync
            if (dto.MinSalary.HasValue && dto.MaxSalary.HasValue && dto.MinSalary > dto.MaxSalary)
                throw new ArgumentException("MinSalary cannot be greater than MaxSalary.");

            if (dto.MinExperience.HasValue && dto.MaxExperience.HasValue && dto.MinExperience > dto.MaxExperience)
                throw new ArgumentException("MinExperience cannot be greater than MaxExperience.");

            if (dto.ClosingDate.HasValue &&
        dto.ClosingDate.Value < dto.PostedDate)
            {
                throw new ArgumentException(
                    "ClosingDate cannot be earlier than PostedDate.");
            }
            var jobPosting = new Jobposting
            {
                Title = dto.Title,
                DepartmentId = dto.DepartmentId,
                MinSalary = dto.MinSalary,
                MaxSalary = dto.MaxSalary,
                EmploymentType = dto.EmploymentType.ToString(),
                WorkMode = dto.WorkMode.ToString(),
                MinExperience = dto.MinExperience,
                MaxExperience = dto.MaxExperience,
                Description = dto.Description,
                Requirements = dto.Requirements,
                Location = dto.Location,
                NumberOfOpenings = dto.NumberOfOpenings,
                PostedDate = dto.PostedDate,
                ClosingDate = dto.ClosingDate,
                CreatedBy = createdBy
                
            };
            var result = await _jobPostingRepo.InsertAsync(jobPosting);
            _logger.LogInformation("Job posting created with ID: {Id}",result.Id);

            return  new JobPostingsReponseDto
            {
                Id = jobPosting.Id,
                Title = jobPosting.Title,
                EmploymentType = jobPosting.EmploymentType,
                WorkMode = jobPosting.WorkMode,
                Status = jobPosting.Status
            } ;

        }

        public async Task DeleteJobPostingAsync(int id, int deletedBy) { 
        
           var jobPosting = await _jobPostingRepo.Table.FirstOrDefaultAsync(j => j.Id == id && j.IsDeleted != true);
            if (jobPosting == null)
                throw new KeyNotFoundException($"Job Posting with ID {id} not found.");

            jobPosting.IsDeleted = true;
            jobPosting.DeletedBy = deletedBy;
            jobPosting.DeletedDate = DateTime.UtcNow;
             await  _jobPostingRepo.UpdateAsync(jobPosting);
            _logger.LogInformation("Job posting deleted with ID: {Id} and JOb: {Title}", id,jobPosting.Title);
           
        }

        public async Task<JobPostingsDetailsDto> GetJobPostingByIdAsync(int id) { 
        
         var jobposting = await _jobPostingRepo.TableNoTracking.FirstOrDefaultAsync(j => j.Id == id && j.IsDeleted != true);
            if (jobposting == null)
                throw new KeyNotFoundException($"Job Posting with ID {id} not found.");

            return new JobPostingsDetailsDto
            {

                Id = jobposting.Id,
                Title = jobposting.Title,
                DepartmentId = jobposting.DepartmentId,
                MinSalary = jobposting.MinSalary,

                MaxSalary = jobposting.MaxSalary,
                EmploymentType = jobposting.EmploymentType,
                WorkMode = jobposting.WorkMode,
                MinExperience = jobposting.MinExperience,
                MaxExperience = jobposting.MaxExperience,
                Description = jobposting.Description,
                Requirements = jobposting.Requirements,
                Location = jobposting.Location,
                NumberOfOpenings = jobposting.NumberOfOpenings,
                Status = jobposting.Status,
                PostedDate = jobposting.PostedDate,
                ClosingDate = jobposting.ClosingDate,


            };

        }

        public async Task<PagedResult<JobPostingsfilterDto>> GetAllJobPostingsAsync(
        JobPostingFilterRequest request)
        {
            var query = _jobPostingRepo.TableNoTracking
                .Where(j => !j.IsDeleted);

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    (j.Description != null && j.Description.Contains(search)) ||
                    (j.Requirements != null && j.Requirements.Contains(search)) ||
                    (j.Location != null && j.Location.Contains(search)));
            }

            // Department
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(j =>
                    j.DepartmentId == request.DepartmentId.Value);
            }

            // Employment Type
            if (request.EmploymentType.HasValue)
            {
                var employmentType = request.EmploymentType.Value.ToString();

                query = query.Where(j =>
                    j.EmploymentType == employmentType);
            }

            // Work Mode
            if (request.WorkMode.HasValue)
            {
                var workMode = request.WorkMode.Value.ToString();

                query = query.Where(j =>
                    j.WorkMode == workMode);
            }

            // Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var status = request.Status.Trim();

                query = query.Where(j =>
                    j.Status == status);
            }

            // Location
            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                var location = request.Location.Trim();

                query = query.Where(j =>
                    j.Location != null &&
                    j.Location.Contains(location));
            }

            // Salary Range
            if (request.MinSalary.HasValue)
            {
                query = query.Where(j =>
                    j.MaxSalary == null ||
                    j.MaxSalary >= request.MinSalary.Value);
            }

            if (request.MaxSalary.HasValue)
            {
                query = query.Where(j =>
                    j.MinSalary == null ||
                    j.MinSalary <= request.MaxSalary.Value);
            }

            // Experience Range
            if (request.MinExperience.HasValue)
            {
                query = query.Where(j =>
                    j.MaxExperience == null ||
                    j.MaxExperience >= request.MinExperience.Value);
            }

            if (request.MaxExperience.HasValue)
            {
                query = query.Where(j =>
                    j.MinExperience == null ||
                    j.MinExperience <= request.MaxExperience.Value);
            }

            // Posted Date From
            if (request.PostedFrom.HasValue)
            {
                query = query.Where(j =>
                    j.PostedDate >= request.PostedFrom.Value);
            }

            // Posted Date To
            if (request.PostedTo.HasValue)
            {
                query = query.Where(j =>
                    j.PostedDate <= request.PostedTo.Value);
            }

            // Closing Date From
            if (request.ClosingFrom.HasValue)
            {
                query = query.Where(j =>
                    j.ClosingDate.HasValue &&
                    j.ClosingDate.Value >= request.ClosingFrom.Value);
            }

            // Closing Date To
            if (request.ClosingTo.HasValue)
            {
                query = query.Where(j =>
                    j.ClosingDate.HasValue &&
                    j.ClosingDate.Value <= request.ClosingTo.Value);
            }


            /*  sorting value: 
       id
       title
      departmentid
      employmenttype
      workmode
      minsalary
      maxsalary
      minexperience
      maxexperience
      status
      posteddate
      closingdate  */
            // Sorting
            var sortOptions =
                new Dictionary<string, Expression<Func<Jobposting, object?>>>
                {
                    ["id"] = j => j.Id,
                    ["title"] = j => j.Title,
                    ["departmentid"] = j => j.DepartmentId,
                    ["employmenttype"] = j => j.EmploymentType,
                    ["workmode"] = j => j.WorkMode,
                    ["minsalary"] = j => j.MinSalary,
                    ["maxsalary"] = j => j.MaxSalary,
                    ["minexperience"] = j => j.MinExperience,
                    ["maxexperience"] = j => j.MaxExperience,
                    ["status"] = j => j.Status,
                    ["posteddate"] = j => j.PostedDate,
                    ["closingdate"] = j => j.ClosingDate
                };

            query = query.ApplySorting(
                request,
                sortOptions,
                defaultSort: j => j.Id);

            // Projection
            var resultQuery = query.Select(j => new JobPostingsfilterDto
            {
                Id = j.Id,

                Title = j.Title,

                DepartmentName = j.Department != null
                    ? j.Department.DepartmentName
                    : null,

                MinSalary = j.MinSalary,
                MaxSalary = j.MaxSalary,

                EmploymentType = j.EmploymentType,
                WorkMode = j.WorkMode,

                MinExperience = j.MinExperience,
                MaxExperience = j.MaxExperience,

                Location = j.Location,

                NumberOfOpenings = j.NumberOfOpenings,

                Status = j.Status,

                PostedDate = j.PostedDate,
                ClosingDate = j.ClosingDate
            });

            // Pagination
            return await resultQuery.ToPagedResultAsync(request);
        }


        public async Task UpdateJobPostingAsync(int id, UpdateJobPostingDto dto, int updatedBy) { 
        
          
            var jobPosting = await _jobPostingRepo.Table.FirstOrDefaultAsync(j => j.Id == id && j.IsDeleted != true);
            if (jobPosting == null)
                throw new KeyNotFoundException($"Job Posting with ID {id} not found.");
            // Update properties
            jobPosting.Title = dto.Title;
            jobPosting.DepartmentId = dto.DepartmentId ?? jobPosting.DepartmentId;
            jobPosting.MinSalary = dto.MinSalary ?? jobPosting.MinSalary;
            jobPosting.MaxSalary = dto.MaxSalary ?? jobPosting.MaxSalary;
            jobPosting.EmploymentType = dto.EmploymentType.ToString();
            jobPosting.WorkMode = dto.WorkMode.ToString();
            jobPosting.MinExperience = dto.MinExperience;
            jobPosting.MaxExperience = dto.MaxExperience;
            jobPosting.Description = dto.Description;
            jobPosting.Requirements = dto.Requirements;
            jobPosting.Location = dto.Location;
            jobPosting.NumberOfOpenings = dto.NumberOfOpenings;
            jobPosting.PostedDate = dto.PostedDate;
            jobPosting.ClosingDate = dto.ClosingDate;
            // Update metadata
            jobPosting.ModifiedBy = updatedBy;
            jobPosting.ModifiedDate = DateTime.UtcNow;
             await _jobPostingRepo.UpdateAsync(jobPosting);
             _logger.LogInformation("Job posting updated with ID: {Id} and Job: {Title}", id,jobPosting.Title);
        }

    }
}