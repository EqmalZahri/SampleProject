using Microsoft.AspNetCore.Mvc;
using Rbac_IctJohor.Models.Dto;
using Rbac_IctJohor.Services;

namespace Rbac_IctJohor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly TenantService _tenantService;

        public TenantController(TenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("CreateTenant")]
        public async Task<IActionResult> CreateTenantAsync([FromBody] RequestCreateTenant input)
        {
            var tenant = await _tenantService.CreateTenantAsync(input);
            return Ok(tenant);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllTenantsAsync()
        {
            var tenants = await _tenantService.GetAllTenantsAsync();
            return Ok(tenants);
        }
    }

}
