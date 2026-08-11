using EMS.Core.DTOs.Careers;
using EMS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Careers
{
    public interface ICareerService
    {
        Task<CareerResponseDto> CreateAsync(CreateCareerDto dto);
        Task<CareerAllResponseDto> GetCandidateDetailsByIdAsync(int id);

        Task DeleteAsync(int id, int DeletedBy);

        Task<PagedResult<CareerCandidatedata>> GetAllAsync(CareerFilterRequest request);

    }
}
