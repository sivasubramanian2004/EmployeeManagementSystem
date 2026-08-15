using EMS.Core.DTOs.JobPostings;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core.Helpers;
namespace EMS.Service.JobPostings
{
    public interface IJobPostingService
    {      
        public Task<JobPostingsReponseDto>  CreateJobPostingAsync(CreateJobPostingDto dto, int createdBy);

        public Task DeleteJobPostingAsync(int id, int deletedBy);

        public Task<JobPostingsDetailsDto> GetJobPostingByIdAsync(int id);

        public Task<PagedResult<JobPostingsfilterDto>> GetAllJobPostingsAsync(JobPostingFilterRequest request);

        public Task UpdateJobPostingAsync(int id, UpdateJobPostingDto dto, int updatedBy);
    }
}
