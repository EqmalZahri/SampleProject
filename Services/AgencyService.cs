using Rbac_IctJohor.Models.Dto;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Rbac_IctJohor.Models.UsersRequest;


namespace Rbac_IctJohor.Services
{
    public class AgencyService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly UserManager<User> _userManager;

        public AgencyService(AppDbContext context, HttpClient httpClient, UserManager<User> userManager)
        {
            _context = context;
            _httpClient = httpClient;
            _userManager = userManager;
        }

        public async Task<Agency> CreateAgencyAsync(RequestCreateAgency input)
        {
            var tagRequest = new TagCreationRequest
            {
                Description = input.TagDescription,
                Name = input.TagName
            };

            // Serialize the tag creation request to JSON
            var tagRequestJson = JsonConvert.SerializeObject(tagRequest);
            var httpContent = new StringContent(tagRequestJson, Encoding.UTF8, "application/json");

            try
            {
                Console.WriteLine($"Serialized JSON: {tagRequestJson}");

                // Make HTTP POST request to create the tag
                var response = await _httpClient.PostAsync("http://localhost:9080/ucm/v1/rest/vsphere/vm/tag", httpContent);
                //var response = await _httpClient.PostAsync("http://konti.cloud-connect.asia:9080/ucm/v1/rest/vsphere/vm/tag", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
                    response.EnsureSuccessStatusCode();
                }

                // Read & deserialize the tag creation response
                var responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response JSON: {responseJson}");
                var tagResponse = JsonConvert.DeserializeObject<TagCreationResponse>(responseJson);

                if (tagResponse == null || string.IsNullOrEmpty(tagResponse.TagId))
                {
                    throw new InvalidOperationException("Failed to retrieve tag ID from the response.");
                }

                
                var agency = new Agency
                {
                    AgencyName = input.AgencyName,
                    AgencyCode = input.AgencyCode,
                    TenantId = input.TenantId,
                    TagId = tagResponse.TagId, 
                    TagName = input.TagName,
                    TagDescription = input.TagDescription,
                    AgencyGroup = input.AgencyGroup,
                    Address = input.Address,
                    AgencyContactNo = input.AgencyContactNo
                };

                
                _context.Agencies.Add(agency);
                await _context.SaveChangesAsync();

                return agency;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<List<AgencyResponse>> GetAllAgenciesAsync()
        {
            return await _context.Agencies
                .Include(a => a.Users)
                .ThenInclude(u => u.Agency)
                .Select(a => new AgencyResponse
                {
                    Id = a.Id,
                    AgencyName = a.AgencyName,
                    AgencyCode = a.AgencyCode,
                    Address = a.Address,
                    AgencyContactNo = a.AgencyContactNo,
                    TagId = a.TagId,
                    TagName = a.TagName,
                    AgencyGroup = a.AgencyGroup,
                    Users = a.Users.Select(u => new UserResponse
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        RoleNames = _context.UserRoles
                            .Where(ur => ur.UserId == u.Id)
                            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                            .ToList(),
                        Email = u.Email
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<AgencyResponse> GetUsersByAgencyIdAsync(Guid agencyId)
        {
            return await _context.Agencies
                .Where(a => a.Id == agencyId)
                .Select(a => new AgencyResponse
                {
                    Id = a.Id,
                    AgencyName = a.AgencyName,
                    AgencyCode = a.AgencyCode,
                    Address = a.Address,
                    AgencyContactNo = a.AgencyContactNo,
                    TagId = a.TagId,
                    TagName = a.TagName,
                    AgencyGroup = a.AgencyGroup,
                    Users = a.Users.Select(u => new UserResponse
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        RoleNames = _context.UserRoles
                            .Where(ur => ur.UserId == u.Id)
                            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                            .ToList(),
                        Email = u.Email
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<Agency> UpdateAgencyAsync(Guid agencyId, UpdateAgencyRequest input)
        {
            var agency = await _context.Agencies.FindAsync(agencyId);
            if (agency == null)
            {
                throw new KeyNotFoundException("Agency Not Found");
            }

            agency.AgencyName = input.AgencyName;
            agency.AgencyCode = input.AgencyCode;
            agency.Address = input.Address;
            agency.AgencyContactNo = input.AgencyContactNo;
            agency.AgencyGroup = input.AgencyGroup;

            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();

            return agency;
        }

        public async Task DeleteAgencyAsync(Guid agencyId)
        {
            var agency = await _context.Agencies.Include(a => a.Users).FirstOrDefaultAsync(a => a.Id == agencyId);
            if (agency == null)
            {
                throw new KeyNotFoundException("Agency Not Found");
            }

            if (agency.Users.Any())
            {
                throw new InvalidOperationException("Cannot Delete Agency With Associate Users");
            }

            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AssignPersonInChargeAsync(RequestAssignPersonInCharge input)
        {
           var user = await _userManager.FindByIdAsync(input.UserId.ToString());
            if (user == null || user.AgencyId != input.AgencyId)
            {
                throw new InvalidOperationException("User not found or does not belong to the specified agency");
            }

            if (user.IsPersonInCharge)
            {
                throw new InvalidOperationException("User Already Been Added As Person In Charge");
            }

            user.IsPersonInCharge = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemovePersonInChargeAsync(RequestAssignPersonInCharge input)
        {
            var user = await _userManager.FindByIdAsync(input.UserId.ToString());
            if (user == null || user.AgencyId != input.AgencyId)
            {
                throw new InvalidOperationException("User not found or does not belong to the specified agency");
            }

            user.IsPersonInCharge = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<RequestUser>> GetPersonInChargeAsync(Guid agencyId)
        {
            var users = await _context.Users
                .Where(u => u.AgencyId == agencyId && u.IsPersonInCharge)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.AgencyId,
                    AgencyName = u.Agency.AgencyName,
                    u.Title,
                    u.IsDisable
                })
                .ToListAsync();

            var requestUsers = new List<RequestUser>();
            foreach (var user in users)
            {
                var roleNames = await _userManager.GetRolesAsync(new User { Id = user.Id });
                requestUsers.Add(new RequestUser
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AgencyId = user.AgencyId,
                    AgencyName = user.AgencyName,
                    Title = user.Title,
                    RoleNames = roleNames.ToList(),
                    IsDisable = user.IsDisable
                });
            }

            return requestUsers;
        }


    }

}
