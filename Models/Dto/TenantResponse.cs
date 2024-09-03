namespace Rbac_IctJohor.Models.Dto
{
    public class TenantResponse
    {
        public Guid Id { get; set; }
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        //public List<AgencyResponse> Agencies { get; set; } = new List<AgencyResponse>();
        public List<AgencyResponse> Agencies { get; set; }
    }

}
