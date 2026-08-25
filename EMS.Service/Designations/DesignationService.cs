using EMS.Core.DTOs.Designation;
using EMS.Data.Models;
using EMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EMS.Service.Designations
{
    public class DesignationService : IDesignationService
    {
        private readonly IRepository<Designation> _designationRepo;
        private readonly ILogger<DesignationService> _logger;

        public DesignationService(IRepository<Designation> designationRepo, ILogger<DesignationService> logger)
        {

            _designationRepo = designationRepo;
            _logger = logger;
        }

        public async Task InsertAsync(CreateDesignationDto dto, int createdBy)
        {
            var existingDesignation = await _designationRepo.Table
                .FirstOrDefaultAsync(d =>
                    d.DesignationName == dto.DesignationName &&
                    d.IsDeleted != true);

            if (existingDesignation != null)
                throw new InvalidOperationException(
                    $"Designation with name '{dto.DesignationName}' already exists.");

            var designation = new Designation
            {
                DesignationName = dto.DesignationName.Trim().ToUpper(),
                CreatedDate = DateTime.UtcNow,
                CreatedBy= createdBy

            };

            var savedDesignation = await _designationRepo.InsertAsync(designation);

            _logger.LogInformation($"New Designation {dto.DesignationName} Created Successfully");
        }

        public async Task<List<DesignationResponseDto>> GetAllAsync()
        {
            return await _designationRepo.TableNoTracking
                .Where(d => d.IsDeleted != true)
                .Select(d => new DesignationResponseDto
            {
                Id = d.Id,
                DesignationName = d.DesignationName
            }).ToListAsync();
        }

        public async Task<DesignationResponseDto> UpdateAsync(int id, CreateDesignationDto dto, int updatedBy) {
            var designation = await _designationRepo.Table.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);
            if (designation == null)
                throw new KeyNotFoundException($"Designation with ID { id } not found.");
            designation.DesignationName = dto.DesignationName.Trim().ToUpper();
            designation.ModifiedDate = DateTime.UtcNow;
            designation.ModifiedBy = updatedBy;
            var result = await _designationRepo.UpdateAsync(designation);
            _logger.LogInformation($"Designation Updated Successfully with Id {id} Designation name {dto.DesignationName}");
            return new DesignationResponseDto
            {
                Id = result.Id,
                DesignationName = result.DesignationName
            };

        }

        public async Task DeleteAsync(int id, int deletedBy) {

            var result = await _designationRepo.Table.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);
            if (result == null)
                throw new KeyNotFoundException($"No Designation found with Id {id}");
            result.IsDeleted = true ;
            result.DeletedDate = DateTime.UtcNow;
            result.DeletedBy = deletedBy;
            var designation = await _designationRepo.UpdateAsync(result);
            _logger.LogInformation($"Designation Deleted Successfully with Id {id} Designation name {result.DesignationName}");
        }
    }
}
