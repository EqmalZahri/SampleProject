using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EZHelper.EfCore;
using EZHelper.EfCore.Pagination;
using Rbac_IctJohor.common;
using Rbac_IctJohor.common.Constants;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Models.UsersRequest;
using Rbac_IctJohor.Models.Dto;
using Rbac_IctJohor.Repositories;

namespace Rbac_IctJohor.Services
{
    public interface IUserService
    {
        Task ChangePasswordAsync(string userId, RequestChangePassword input, CancellationToken cancellationToken);
        Task ChangePasswordByIdAsync(string userId, ChangePasswordById input, CancellationToken cancellationToken);
        Task<bool> CheckChangePassword(string userId, CancellationToken cancellationToken);
        Task<bool> CheckProfilesync(Guid id, CancellationToken cancellationToken);   
        Task<RequestUser> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<RequestUser> UpdateUserAsync(Guid id, RequestUpdateUserProfile input, CancellationToken cancellationToken);
        Task DisableUserAsync(string id, CancellationToken cancellationToken);
        Task<List<RequestUser>> ListAllUsersAsync(CancellationToken cancellationToken);
        //Task<PaginationList<User>> GetAllAsync(PaginationUserRequest input, ISessionManager sessionManager, CancellationToken cancellationToken);
        Task<IdentityResult> CreateUserAsync(ReqCreateUser input, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
        //Task ResetPasswordAsync(string userId, CancellationToken cancellationToken);

    }
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly AppSettings _options;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly RoleManager<Role> _roleManager;
        //private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public UserService(AppDbContext db, IOptions<AppSettings> options, UserManager<User> userManager, ILogger<UserService> logger,
            RoleManager<Role> roleManager)
        {
            _db = db;
            _options = options.Value;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
            
        }

        public async Task ChangePasswordAsync(string userId, RequestChangePassword input, CancellationToken cancellationToken)
        {
            Guid userIdParse = Guid.Parse(userId);
            User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userIdParse, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User {userId} Does Not Exist !");
            }

            var isPasswordRecycle = await _userManager.CheckPasswordAsync(user, input.NewPassword);
            if (isPasswordRecycle)
            {
                throw new Exception("Cannot Change To Same Password, Please Use A Different Password");
            }

            var passwordValidator = new PasswordValidator<User>();
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, input.NewPassword);
            if (!passwordValidationResult.Succeeded)
            {
                throw new Exception(passwordValidationResult.Errors.FirstOrDefault()?.Description);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, input.CurrentPassword,
                input.NewPassword);
            if (result.Succeeded == false)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }
            user.ChangePassword = false;
            _db.Users.Update(user);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordByIdAsync(string userId, ChangePasswordById input, CancellationToken cancellationToken)
        {
            if (input.NewPassword != input.ConfirmNewPassword)
            {
                throw new Exception("Passwords do not match.");
            }

            Guid userIdParse = Guid.Parse(userId);
            User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userIdParse, cancellationToken);
            if (user == null)
            {
                throw new Exception($"User {userId} does not exist.");
            }

            var isPasswordRecycle = await _userManager.CheckPasswordAsync(user, input.NewPassword);
            if (isPasswordRecycle)
            {
                throw new Exception("Cannot change to the same password, please use a different password.");
            }

            var passwordValidator = new PasswordValidator<User>();
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, input.NewPassword);
            if (!passwordValidationResult.Succeeded)
            {
                throw new Exception(passwordValidationResult.Errors.FirstOrDefault()?.Description);
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, input.NewPassword);
            user.ChangePassword = false;
            _db.Users.Update(user);
            await _db.SaveChangesAsync(cancellationToken);
        }


        public async Task<bool> CheckChangePassword(string userId, CancellationToken cancellationToken)
        {
            var parseUserId = Guid.Parse(userId);
            var result = await _db.Users.AsNoTracking().
                            AnyAsync(a => a.Id == parseUserId && a.ChangePassword,
                            cancellationToken);
            return result;
        }

        public async Task<bool> CheckProfilesync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Users.AsNoTracking().
                    AnyAsync(x => x.Id == id && (string.IsNullOrWhiteSpace(x.UserName) ||
                    string.IsNullOrWhiteSpace(x.Email) ||
                    string.IsNullOrWhiteSpace(x.PhoneNumber)),
                    cancellationToken);
        }

        public async Task<RequestUser> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _db.Users
                .Where(a => a.Id == id)
                .Select(a => new RequestUser
                {
                    Id = a.Id,
                    Username = a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    AgencyId = a.AgencyId,
                    AgencyName = _db.Agencies
                        .Where(agency => agency.Id == a.AgencyId)
                        .Select(agency => agency.AgencyName)
                        .FirstOrDefault(),
                    Title = a.Title,
                    RoleNames = _db.UserRoles
                        .Where(ur => ur.UserId == a.Id)
                        .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()
                }).FirstOrDefaultAsync();
        }


        public async Task<RequestUser> UpdateUserAsync(Guid id, RequestUpdateUserProfile input, CancellationToken cancellationToken)
        {
            var user = await _db.Users
                     .FirstOrDefaultAsync(x => x.Id == id,
                 cancellationToken);
            if (user is null)
            {
                throw new Exception($"User Id {id} does not exist");
            }
            user.UserName = input.UserName;
            user.Email = input.Email;
            user.NormalizedEmail = input.Email.ToUpper();
            user.PhoneNumber = input.PhoneNumber;
            user.Title = input.Title;
            user.AgencyId = input.AgencyId;
            
            _db.Users.Update(user);
            await _db.SaveChangesAsync(cancellationToken);
            return new RequestUser
            {
                Email = user.Email,
                Username = user.UserName,
                Id = id,
                PhoneNumber = user.PhoneNumber,
                AgencyId = user.AgencyId,
                Title = user.Title
            };

        }
     
        public async Task DisableUserAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                Guid Idparse = Guid.Parse(id);
                User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == Idparse);
                if (id == null)
                {
                    throw new Exception($"User Id {id} does not exist");
                }
                var resp = await _userManager.FindByIdAsync(id);
                resp.IsDisable = true;
                _db.Users.Update(user);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<RequestUser>> ListAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _db.Users
                .Select(a => new RequestUser
                {
                    Id = a.Id,
                    Username = a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    AgencyId = a.AgencyId,
                    AgencyName = _db.Agencies
                        .Where(agency => agency.Id == a.AgencyId)
                        .Select(agency => agency.AgencyName)
                        .FirstOrDefault(),
                    Title = a.Title,
                    RoleNames = _db.UserRoles
                        .Where(ur => ur.UserId == a.Id)
                        .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList(),
                    /*IsDisable = a.IsDisabled */// Assuming there's an IsDisabled property in your User entity
                }).ToListAsync(cancellationToken);
        }

        //public async Task<PaginationList<User>> GetAllAsync(PaginationUserRequest input, ISessionManager sessionManager, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var filter = _db.Users.AsQueryable();
        //        //var filter = _db.Users.Include(u => u.Roles).AsQueryable();

        //        if (input.FilterDisable == true)
        //        {
        //            filter = filter.Where(x => x.IsDisable == input.FilterDisable);
        //        }
        //        var result = await filter.PaginateAsync<User>(input, cancellationToken);
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}



        public async Task<IdentityResult> CreateUserAsync(ReqCreateUser input, CancellationToken cancellationToken)
        {

            var existingUser = await _userManager.FindByEmailAsync(input.Email);
            if (existingUser != null)
            {
                // Email already exists, return failure result
                var errorResult = IdentityResult.Failed(new IdentityError { Description = "Email is already registered/existed." });
                return errorResult;
            }

            var newUsers = new User
            {
                UserName = input.UserName,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber,
                //JobTitleId = input.JobTitleId,
                AgencyId = input.AgencyId,
                Title = input.Title
                //ChangePassword = true,  
            };

            var result = await _userManager.CreateAsync(newUsers, input.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            foreach (var roleName in input.RoleNames)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new Role(roleName);
                    //var role = new IdentityRole<Guid>(roleName);
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        return roleResult;
                    }
                }
                await _userManager.AddToRoleAsync(newUsers, roleName);
            }

            return IdentityResult.Success;

        }

        public async Task<IdentityResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"User with ID {id} does not exist." });
            }

            var result = await _userManager.DeleteAsync(user);
            return result;
        }


        //public async Task ResetPasswordAsync(string userId, CancellationToken cancellationToken)
        //{
        //    Guid userIdParse = Guid.Parse(userId);
        //    User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userIdParse, cancellationToken);
        //    if (userId == null)
        //    {
        //        throw new Exception($"User Id {userId} does not exist");
        //    }
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var resp = await _userManager.ResetPasswordAsync(user, token, _options.SeedSettings.DefaultPassword);
        //    if (resp.Succeeded == false)
        //    {
        //        throw new Exception(resp.Errors.FirstOrDefault()?.Description);
        //    }
        //    user.ChangePassword = true;
        //    _db.Users.Update(user);
        //    await _db.SaveChangesAsync(cancellationToken);
        //}
    }
}
