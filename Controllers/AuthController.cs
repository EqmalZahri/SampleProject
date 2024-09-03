using Rbac_IctJohor.common;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Models.Authentication;
using Rbac_IctJohor.Models.UsersRequest;
using Rbac_IctJohor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Rbac_IctJohor.Repositories;
using Rbac_IctJohor.common.Constants;
using Microsoft.EntityFrameworkCore;


namespace Rbac_IctJohor.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly IJobTitleService _jobTitleService;
        private readonly IUserService _userService;
        private readonly ISessionManager _sessionManager;
        private readonly AppDbContext _dbContext;
        private readonly RoleManager<Role> _roleManager;
        public AuthController(SignInManager<User> signInManager,
            IAuthService authService, UserManager<User> userManager,
        IJobTitleService jobTitleService, IUserService userService, ISessionManager sessionManager, AppDbContext dbContext, RoleManager<Role> roleManager)
        {
            _signInManager = signInManager;
            _authService = authService;
            _userManager = userManager;
            _jobTitleService = jobTitleService;
            _userService = userService;
            _sessionManager = sessionManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        //[HttpPost("[action]")]
        //public async Task<IActionResult> Login([FromBody] RequestAuthentication input,
        //    CancellationToken cancellationToken)
        //{
        //    var result = await _signInManager.PasswordSignInAsync(input.Username, input.Password,
        //        false, false);
        //    if (result.Succeeded == false)
        //    {
        //        //TODO: change to BM or do localization
        //        return BadRequest("Invalid Username or Password");
        //    }
        //    var user = await _userManager.FindByNameAsync(input.Username);
        //    if (user.IsDisable == true)
        //    {
        //        return BadRequest("Your Account Is Disabled");
        //    }
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var jobTitle = new JobTitle();

        //    if (user.JobTitleId is not null)
        //    {
        //        jobTitle = await _jobTitleService.GetByIdAsync(user.JobTitleId.Value, cancellationToken);
        //    }

        //    var accessToken = _authService.CreateAccessToken(user.Id.ToString(), user.UserName,
        //        roles, jobTitle is null ? ""
        //        : jobTitle.Name);

        //    return Ok(accessToken);
        //}

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] RequestAuthentication input,
     CancellationToken cancellationToken)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
            {
                return BadRequest("Invalid Email");
            }

            // Attempt to sign in with the provided email and password
            var result = await _signInManager.PasswordSignInAsync(user.UserName, input.Password, false, false);
            if (result.Succeeded == false)
            {
                //TODO: change to BM or do localization
                return BadRequest("Invalid Password");
            }

            if (user.IsDisable == true)
            {
                return BadRequest("Your Account Is Disabled");
            }

            var roles = await _userManager.GetRolesAsync(user);
            //var jobTitle = new JobTitle();

            //if (user.JobTitleId is not null)
            //{
            //    jobTitle = await _jobTitleService.GetByIdAsync(user.JobTitleId.Value, cancellationToken);
            //}

            var agency = await _dbContext.Agencies
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(a => a.Id == user.AgencyId, cancellationToken);

            if (agency == null)
            {
                return BadRequest("Agency Not Found");
            }

            var tenant = agency.Tenant;

            var accessToken = _authService.CreateAccessToken(
                user.Id.ToString(), 
                user.UserName,
                roles, 
                //jobTitle is null ? "" : jobTitle.Name, 
                user.Email,
                tenant.TenantName,
                tenant.TenantCode,
                agency.AgencyName,
                agency.AgencyCode,
                user.AgencyId.ToString()
                );

            return Ok(accessToken);
        }



        [HttpGet("CheckChangePassword")]
        [Authorize]
        public async Task<IActionResult> CheckChangePassword(CancellationToken cancellationToken)
        {
            bool result = await _userService.CheckChangePassword(_sessionManager.UserId,
                cancellationToken);
            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(RequestChangePassword input, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.ChangePasswordAsync(_sessionManager.UserId, input,cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[HttpPost("SeedData")]

        //public async Task<IActionResult> SeedData()
        //{
        //    var user = new User { UserName = "DummyAccount" };

        //    await _userManager.CreateAsync(user,"123qwe");

        //    var role = new Role { Name = RoleConst.UCMP_Administrator };

        //    await _roleManager.CreateAsync(role);

        //    await _userManager.AddToRoleAsync(user, RoleConst.UCMP_Administrator);

        //    return Ok();
        //}

        [HttpPost("SeedData")]
        public async Task<IActionResult> SeedData()
        {
            // Create Tenant
            var tenant = new Tenant
            {
                TenantName = "Pejabat Setiausaha Kerajaan Johor",
                TenantCode = "ICT_Johor"
            };

            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();

            // Create Agency
            var agency = new Agency
            {
                AgencyName = "Majlis Pembandaran Johor Bahru",
                AgencyCode = "MPJB",
                TenantId = tenant.Id
            };

            _dbContext.Agencies.Add(agency);
            await _dbContext.SaveChangesAsync();

            // Create User
            var user = new User
            {
                UserName = "EqmalZahri",
                Email = "eqmalzahri@gmail.com",
                PhoneNumber = "0124314227",
                //JobTitleId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                AgencyId = agency.Id,
                //ChangePassword = true  // Assuming you want the user to change password on first login
            };

            var userCreationResult = await _userManager.CreateAsync(user, "123qwe");
            if (!userCreationResult.Succeeded)
            {
                return BadRequest(userCreationResult.Errors);
            }

            // Ensure roles exist and assign roles to user
            var roleNames = new[] { "UcmpAdministrator", "CloudViewer" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new Role(roleName); // Assuming Role is your custom role class
                    var roleCreationResult = await _roleManager.CreateAsync(role);
                    if (!roleCreationResult.Succeeded)
                    {
                        return BadRequest(roleCreationResult.Errors);
                    }
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addToRoleResult.Succeeded)
                {
                    return BadRequest(addToRoleResult.Errors);
                }
            }

            return Ok();
        }

    }
}
