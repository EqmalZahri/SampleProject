using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EZHelper.EfCore;
using EZHelper.EfCore.Pagination;
using Rbac_IctJohor.common;
using Rbac_IctJohor.common.Constants;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Repositories;

namespace Rbac_IctJohor.Services
{
    public class RoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' already exists." });
            }

            var role = new Role(roleName);
            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        public async Task<IdentityResult> DeleteRoleAsync(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role with ID {id} does not exist." });
            }

            var result = await _roleManager.DeleteAsync(role);
            return result;
        }

        public async Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
            return roles;
        }

        public async Task<Role> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            return role;
        }

        public async Task<IList<string>> GetRolesByUserIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> UpdateUserRolesAsync(Guid userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(roles).ToList();

            var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!result.Succeeded)
            {
                return result;
            }

            result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            return result;
        }
    }
}
