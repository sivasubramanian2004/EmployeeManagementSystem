using EMS.Core.DTOs.Departments;
using EMS.Core.DTOs.Designation;
using EMS.Data.Models;
using EMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Departments
{
    public class DepartmentService: IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepo;
        private ILogger<DepartmentService> _logger;
        public DepartmentService(IRepository<Department> departmentRepo, ILogger<DepartmentService> logger) {

            _departmentRepo = departmentRepo;
             _logger = logger;

        }

        public async Task InsertAsync(CreateDepartmentDto dto, int CreatedBy) {

            var existsDepartment = await _departmentRepo.Table.FirstOrDefaultAsync(d => d.DepartmentName == dto.DepartmentName && d.IsDeleted != true);
            if (existsDepartment != null)
                throw new InvalidOperationException($"Department {dto.DepartmentName} already existed");
            var department = new Department
            {
                DepartmentName = dto.DepartmentName.Trim().ToUpper(),
                CreatedDate = DateTime.Now,
                CreatedBy = CreatedBy,

            };
            var result = await _departmentRepo.InsertAsync(department);
            _logger.LogInformation($"Department {dto.DepartmentName} Added Succesfully");
          
           
        }

        public async Task DeleteAsync(int id, int DeletedBy) {
            var department = await _departmentRepo.Table.
                                 FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);
            if (department == null)
                throw new KeyNotFoundException($"No Department found with ID {id}");

            department.IsDeleted = true;
            department.DeletedDate = DateTime.UtcNow;
            department.DeletedBy = DeletedBy;

           
            var result =await _departmentRepo.UpdateAsync(department);
            _logger.LogInformation($"Deleted Department {department.DepartmentName}");
        }
        public async Task<DesignationResponseDto> UpdateAsync(int id, CreateDepartmentDto dto, int UpdatedBy) {

            var department = await _departmentRepo.Table.
                                  FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);
            if (department == null)
                throw new KeyNotFoundException($"No Department found with ID {id}");
            department.DepartmentName = dto.DepartmentName.Trim().ToUpper();
            department.ModifiedDate = DateTime.UtcNow;
            department.ModifiedBy = UpdatedBy;
            var result = await _departmentRepo.UpdateAsync(department);
            _logger.LogInformation($"Department Id {id} Updated Successfully");
            return new DesignationResponseDto
            {
                Id = department.Id,
                DesignationName = dto.DepartmentName,
            };

        }
        public async Task<List<DesignationResponseDto>> GetAllAsync() {

            var department = await _departmentRepo.TableNoTracking
                .Where(d=>d.IsDeleted!=true).OrderBy(d=>d.Id).ToListAsync();
            
            return department.Select(d=> new DesignationResponseDto{
             Id = d.Id,
             DesignationName=d.DepartmentName
            
            }).ToList();
        
        
        }
    }
}
