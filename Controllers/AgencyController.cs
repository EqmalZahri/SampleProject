using Microsoft.AspNetCore.Mvc;
using Rbac_IctJohor.Models.Dto;
using Rbac_IctJohor.Services;

namespace Rbac_IctJohor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly AgencyService _agencyService;

        public AgencyController(AgencyService agencyService)
        {
            _agencyService = agencyService;
        }

        [HttpPost("CreateAgencies")]
        public async Task<IActionResult> CreateAgencyAsync([FromBody] RequestCreateAgency input)
        {
            var agency = await _agencyService.CreateAgencyAsync(input);
            return Ok(agency);
        }

        [HttpGet("GetAllAgencies")]
        public async Task<IActionResult> GetAllAgencies()
        {
            var agencies = await _agencyService.GetAllAgenciesAsync();
            if (agencies == null || !agencies.Any())
            {
                return NotFound();
            }
            return Ok(agencies);
        }

        [HttpGet("GetAgenciesById")]
        public async Task<IActionResult> GetUsersByAgency(Guid agencyId)
        {
            var agencyResponse = await _agencyService.GetUsersByAgencyIdAsync(agencyId);
            if (agencyResponse == null)
            {
                return NotFound();
            }
            return Ok(agencyResponse);
        }

        [HttpPut("UpdateAgency/{id}")]
        public async Task<IActionResult> UpdateAgency(Guid id, [FromBody] UpdateAgencyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedAgency = await _agencyService.UpdateAgencyAsync(id, request);
                return Ok(updatedAgency);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteAgency/{id}")]
        public async Task<IActionResult> DeleteAgency(Guid id)
        {
            try
            {
                await _agencyService.DeleteAgencyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Assign-pic")]
        public async Task<IActionResult> AssignPersonInCharge([FromBody] RequestAssignPersonInCharge request)
        {
            try
            {
                var result = await _agencyService.AssignPersonInChargeAsync(request);
                if (result)
                {
                    return Ok(new { Message = "Person In Charge Assigned Successfully" });
                }

                return BadRequest(new { Message = "Failed To Assign Person In Charge" });
            }
            catch (InvalidOperationException ex) when (ex.Message == "User Already Been Added As Person In Charge")
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("Remove-pic")]
        public async Task<IActionResult> RemovePersonInCharge([FromBody] RequestAssignPersonInCharge request)
        {
            try
            {
                var result = await _agencyService.RemovePersonInChargeAsync(request);
                if (result)
                {
                    return Ok(new { Message = "Person In Charge Removed Successfully" });
                }

                return BadRequest(new { Message = "Failed To Remove Person In Charge" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("get-pics/{agencyId}")]
        public async Task<IActionResult> GetPersonsInCharge(Guid agencyId)
        {
            try
            {
                var pics = await _agencyService.GetPersonInChargeAsync(agencyId);
                return Ok(pics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

    }
}
