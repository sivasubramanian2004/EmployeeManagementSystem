using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core.DTOs.Designation;
namespace EMS.Service.Designations
{
    public interface IDesignationService
    {
        Task<List<DesignationResponseDto>> GetAllAsync();
        Task InsertAsync(CreateDesignationDto dto,int createdBy);
        Task<DesignationResponseDto> UpdateAsync(int id, CreateDesignationDto dto, int updatedBy);
        Task DeleteAsync(int id, int deletedBy);

    }
}
