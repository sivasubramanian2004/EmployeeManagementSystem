using DocumentFormat.OpenXml.Office2010.Excel;
using EMS.Core.DTOs.Documents;
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

        Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentDto dto, int uploadedBy);
        Task<EmployeeFullDetailsDto?> GetEmployeeFullDetailsByIdAsync(int employeeId);

        Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(EmployeeFilterRequest request);

        Task  DeleteAsync(int id, int deletedBy);

        Task<EmployeeResponseDto>  UpdateEmployeeAsync(int id, UpdateEmployeeDto dto, int updatedBy);
    }

}