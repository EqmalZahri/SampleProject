using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Services;

namespace Rbac_IctJohor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] string roleName, CancellationToken cancellationToken)
        {
            var result = await _roleService.CreateRoleAsync(roleName, cancellationToken);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _roleService.DeleteRoleAsync(id, cancellationToken);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleService.GetAllRolesAsync(cancellationToken);
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetRoleByIdAsync(id, cancellationToken);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [HttpGet("user/{userId}/roles")]
        public async Task<IActionResult> GetRolesByUserId(Guid userId)
        {
            try
            {
                var roles = await _roleService.GetRolesByUserIdAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("user/{userId}/roles")]
        public async Task<IActionResult> UpdateUserRoles(Guid userId, [FromBody] List<string> roles)
        {
            try
            {
                var result = await _roleService.UpdateUserRolesAsync(userId, roles);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }

    //public class RoleController : ControllerBase
    //{
    //    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    //    public RoleController(RoleManager<IdentityRole<Guid>> roleManager)
    //    {
    //        _roleManager = roleManager;
    //    }

    //    [HttpPost("create")]
    //    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    //    {
    //        if (string.IsNullOrWhiteSpace(roleName))
    //        {
    //            return BadRequest("Role name should not be empty.");
    //        }

    //        var roleExist = await _roleManager.RoleExistsAsync(roleName);
    //        if (!roleExist)
    //        {
    //            var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
    //            if (roleResult.Succeeded)
    //            {
    //                return Ok("Role created successfully.");
    //            }
    //            return BadRequest("Role creation failed.");
    //        }
    //        return BadRequest("Role already exists.");
    //    }
    //}
}
