namespace Rbac_IctJohor.Models.Dto
{
    public class RequestCreateAgency
    {
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public Guid TenantId { get; set; }
        public string AgencyContactNo { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }

        //public string AgencyGroup { get; set; }
        public List<string> AgencyGroup { get; set; } = new List<string>();

        public string Address { get; set; }
    }

}
