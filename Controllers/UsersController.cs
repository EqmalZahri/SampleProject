using Rbac_IctJohor.common;
using Rbac_IctJohor.Models.UsersRequest;
using Rbac_IctJohor.Models.Dto;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Services;
using EZHelper.EfCore.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rbac_IctJohor.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISessionManager _sessionManager;

        public UsersController(IUserService userService, ISessionManager sessionManager)
        {
            _userService = userService;
            _sessionManager = sessionManager;
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> AddUserAsync(ReqCreateUser input, CancellationToken cancellationToken)
        {
            var result = await _userService.CreateUserAsync(input, cancellationToken);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, RequestUpdateUserProfile input,
            CancellationToken cancellationToken)
        {
            RequestUser user = await _userService.UpdateUserAsync(id, input, cancellationToken);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteUserAsync(id, cancellationToken);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }


        //[HttpGet("All")]
        //public async Task<IActionResult> GetAllUser([FromQuery] PaginationUserRequest input, CancellationToken cancellationToken)
        //{
        //    PaginationList<User> result = await _userService.GetAllAsync(input, _sessionManager, cancellationToken);
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<List<RequestUser>>> ListAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.ListAllUsersAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            RequestUser user = await _userService.GetUserByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpPost("change-password-by-id/{userId}")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordById(string userId, [FromBody] ChangePasswordById model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _userService.ChangePasswordByIdAsync(userId, model, cancellationToken);
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/CheckProfile")]
        public async Task<IActionResult> CheckProfile(Guid id, CancellationToken cancellationToken)
        {
            bool result = await _userService.CheckProfilesync(id, cancellationToken);
            return Ok(result);
        }

        

        [HttpPut("Disable/{id}")]
        public async Task<IActionResult> DisableUser(string id, CancellationToken cancellationToken)
        {
            await _userService.DisableUserAsync(id, cancellationToken);
            return Ok();
        }

        //[HttpPost("bulk")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Bulk(IFormFile input, CancellationToken cancellationToken)
        //{
        //    await _userService.CreateAsync(input, cancellationToken);
        //    return Ok();
        //}

        //[HttpPost("ResetPassword/{userId}")]
        //public async Task<IActionResult> ResetPassword(string userId, CancellationToken cancellationToken)
        //{
        //    await _userService.ResetPasswordAsync(userId, cancellationToken);
        //    return Ok();
        //}


    }
}
