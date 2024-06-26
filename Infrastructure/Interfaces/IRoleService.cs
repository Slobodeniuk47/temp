using Infrastructure.Models.DTO.RoleDTO;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IRoleService
    {
        Task<ServiceResponse> GetAllRolesAsync();
        Task<ServiceResponse> GetRoleByIdAsync(long id);
        Task<ServiceResponse> CreateRoleAsync(RoleCreateDTO model);
        Task<ServiceResponse> EditRoleAsync(RoleEditDTO model);
        Task<ServiceResponse> DeleteRoleByIdAsync(long id);
    }
}
