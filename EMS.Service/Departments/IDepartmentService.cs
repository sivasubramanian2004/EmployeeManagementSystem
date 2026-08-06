using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Core.DTOs.Departments;
using EMS.Core.DTOs.Designation;

namespace EMS.Service.Departments
{
    public interface IDepartmentService
    {
        Task InsertAsync(CreateDepartmentDto dto, int CreatedBy);
        Task DeleteAsync(int id, int DeletedBy);
        Task<DesignationResponseDto> UpdateAsync(int id, CreateDepartmentDto dto, int DeletedBy);

        Task<List<DesignationResponseDto>> GetAllAsync();
    }
}
