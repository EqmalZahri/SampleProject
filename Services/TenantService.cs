using Rbac_IctJohor.Models.Dto;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Rbac_IctJohor.Services
{
    public class TenantService
    {
        private readonly AppDbContext _context;

        public TenantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant> CreateTenantAsync(RequestCreateTenant input)
        {
            var tenant = new Tenant
            {
                TenantName = input.TenantName,
                TenantCode = input.TenantCode
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return tenant;
        }

        //public async Task<List<TenantResponse>> GetAllTenantsAsync()
        //{
        //    return await _context.Tenants
        //        .Include(t => t.Agencies)
        //        .ThenInclude(a => a.Users)
        //        .Select(t => new TenantResponse
        //        {
        //            Id = t.Id,
        //            TenantName = t.TenantName,
        //            TenantCode = t.TenantCode,
        //            Agencies = t.Agencies.Select(a => new AgencyResponse
        //            {
        //                Id = a.Id,
        //                AgencyName = a.AgencyName,
        //                AgencyCode = a.AgencyCode,
        //                Users = a.Users.Select(u => new UserResponse
        //                {
        //                    Id = u.Id,
        //                    UserName = u.UserName,
        //                    RoleName = _context.UserRoles
        //                        .Where(ur => ur.UserId == u.Id)
        //                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
        //                        .FirstOrDefault(),
        //                    Email = u.Email
        //                }).ToList()
        //            }).ToList()
        //        }).ToListAsync();
        //}

        public async Task<List<TenantResponse>> GetAllTenantsAsync()
        {
            return await _context.Tenants
                .Include(t => t.Agencies)
                .ThenInclude(a => a.Users)
                .Select(t => new TenantResponse
                {
                    Id = t.Id,
                    TenantName = t.TenantName,
                    TenantCode = t.TenantCode,
                    Agencies = t.Agencies.Select(a => new AgencyResponse
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
                                .ToList(), // Fetch all roles
                            Email = u.Email
                        }).ToList()
                    }).ToList()
                }).ToListAsync();
        }

    }

}
