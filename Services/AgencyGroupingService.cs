using Rbac_IctJohor.Models;
using Rbac_IctJohor.Repositories;
using Microsoft.EntityFrameworkCore;
using Rbac_IctJohor.Models.Dto;

namespace Rbac_IctJohor.Services
{
    public class AgencyGroupingService
    {
        private readonly AppDbContext _context;

        public AgencyGroupingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AgencyGrouping> CreateAgencyGroupingAsync(RequestCreateAgencyGrouping input)
        {

            if (await _context.AgencyGroupings.AnyAsync(ag => ag.GroupName == input.GroupName))
            {
                throw new Exception("Group Already Exists");
            }

            var agencyGrouping = new AgencyGrouping
            {
                GroupName = input.GroupName,
                GroupDesc = input.GroupDesc
            };

            _context.AgencyGroupings.Add(agencyGrouping);
            await _context.SaveChangesAsync();

            return agencyGrouping;
        }

        public async Task<List<AgencyGrouping>> GetAllAgencyGroupingAsync()
        {
            return await _context.AgencyGroupings.ToListAsync();


        }

        //public async Task<bool> DeleteAgencyGroupingAsync(string groupName)
        //{
        //    // Fetch all Agency entities with AgencyGroup containing groupName
        //    var agenciesWithGroup = await _context.Agencies
        //        .Where(a => a.AgencyGroup.Any(ag => ag == groupName))
        //        .ToListAsync();

        //    // Check if any Agency uses this groupName
        //    bool isGroupUsed = agenciesWithGroup.Any();
        //    if (isGroupUsed)
        //    {
        //        return false; // Group is in use, cannot delete
        //    }

        //    // Get the AgencyGrouping by groupName
        //    var agencyGrouping = await _context.AgencyGroupings.FirstOrDefaultAsync(ag => ag.GroupName == groupName);
        //    if (agencyGrouping == null)
        //    {
        //        return false; // Group not found
        //    }

        //    // Delete the AgencyGrouping
        //    _context.AgencyGroupings.Remove(agencyGrouping);
        //    await _context.SaveChangesAsync();

        //    return true; // Successfully deleted
        //}

    }



}
