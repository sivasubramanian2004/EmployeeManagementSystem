using EMS.Core.DTOs.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service.Roles
{
    public interface IRoleService
    {

        Task<List<RoleResponseDto>> GetAllAsync();
        Task InsertAsync(CreateRoleDto dto, int createdBy);
        Task<RoleResponseDto> UpdateAsync(int id, CreateRoleDto dto, int updatedBy);
        Task DeleteAsync(int id, int deletedBy);
    }
}
