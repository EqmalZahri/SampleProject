using Microsoft.AspNetCore.Mvc;
using Rbac_IctJohor.Services;
using Rbac_IctJohor.Models;
using Rbac_IctJohor.Models.Dto;

namespace Rbac_IctJohor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AgencyGroupingController : ControllerBase
    {
        private readonly AgencyGroupingService _agencyGroupingService;

        public AgencyGroupingController(AgencyGroupingService agencyGroupingService)
        {
            _agencyGroupingService = agencyGroupingService;
        }

        [HttpPost("CreateAgencyGrouping")]
        public async Task<IActionResult> CreateAgencyGroupingAsync([FromBody] RequestCreateAgencyGrouping input)
        {
            try
            {
                var agencyGrouping = await _agencyGroupingService.CreateAgencyGroupingAsync(input);
                return Ok(agencyGrouping);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAgencyGroupingAsync()
        {
            var agencyGrouping = await _agencyGroupingService.GetAllAgencyGroupingAsync();
            return Ok(agencyGrouping);
        }

        //[HttpDelete("DeleteAgencyGroup/{groupName}")]
        //public async Task<IActionResult> DeleteAgencyGrouping(string groupName)
        //{
        //    bool result = await _agencyGroupingService.DeleteAgencyGroupingAsync(groupName);

        //    if (!result)
        //    {
        //        return BadRequest(new { message = "GroupName is in use or does not exist" });
        //    }

        //    return Ok(new { message = "GroupName deleted successfully" });
        //}

    }


}
