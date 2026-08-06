using EMS.Core.DTOs.Roles;
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
namespace EMS.Service.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepo;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRepository<Role> roleRepo, ILogger<RoleService> logger)
        {

            _roleRepo =roleRepo;
            _logger = logger;
        }

        public async Task InsertAsync(CreateRoleDto dto, int createdBy)
        {
            var existingRole = await _roleRepo.Table
                .FirstOrDefaultAsync(d =>
                    d.RoleName == dto.RoleName &&
                    d.IsDeleted != true);

            if (existingRole != null)
                throw new InvalidOperationException(
                    $"Role with name '{dto.RoleName}' already exists.");

            var role = new Role
            {
                RoleName = dto.RoleName,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy

            };

            var savedRole = await _roleRepo.InsertAsync(role);

            _logger.LogInformation($"New Role {dto.RoleName} Created Successfully");
        }

        public async Task<List<RoleResponseDto>> GetAllAsync()
        {
            var role = await _roleRepo.TableNoTracking
                .Where(d => d.IsDeleted != true)
                .ToListAsync();

            return role.Select(d => new RoleResponseDto
            {
                Id = d.Id,
                RoleName = d.RoleName
            }).ToList();
        }

        public async Task<RoleResponseDto> UpdateAsync(int id, CreateRoleDto dto, int updatedBy)
        {
            var role = await _roleRepo.Table.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} not found.");
            role.RoleName = dto.RoleName.Trim();
            role.ModifiedDate = DateTime.UtcNow;
            role.ModifiedBy = updatedBy;
            var result = await _roleRepo.UpdateAsync(role);
            _logger.LogInformation($"Role Updated Successfully with Id {id} Role name {dto.RoleName}");
            return new RoleResponseDto
            {
                Id = result.Id,
                RoleName = result.RoleName
            };

        }

        public async Task DeleteAsync(int id, int deletedBy)
        {

            var result = await _roleRepo.Table.FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted != true);
            if (result == null)
                throw new KeyNotFoundException($"No role found with Id {id}");
            result.IsDeleted = true;
            result.DeletedDate = DateTime.UtcNow;
            result.DeletedBy = deletedBy;
            var role = await _roleRepo.UpdateAsync(result);
            _logger.LogInformation($"Role Deleted Successfully with Id {id} Role name {result.RoleName}");
        }
    }
}
