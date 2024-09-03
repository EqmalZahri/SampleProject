namespace Rbac_IctJohor.Models.Dto
{
    public class UpdateAgencyRequest
    {
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string Address { get; set; }
        public string AgencyContactNo { get; set; }
        public List<string> AgencyGroup { get; set; } = new List<string>();
    }
}
