using EMS.Core.DTOs.Employees;
using EMS.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Employees
{
    public interface IEmployeeService
    {
        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto, int createdBy);
        Task<EmployeeFullDetailsDto?> GetEmployeeFullDetailsByIdAsync(int employeeId);

        Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(PaginationRequest request);
    }

}