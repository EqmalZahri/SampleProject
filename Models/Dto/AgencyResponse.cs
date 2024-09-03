namespace Rbac_IctJohor.Models.Dto
{
    public class AgencyResponse
    {
        public Guid Id { get; set; }
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string Address { get; set; }
        public string AgencyContactNo { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
        //public string AgencyGroup { get; set; }
        public List<string> AgencyGroup { get; set; } = new List<string>();
        public List<UserResponse> Users { get; set; }
        
    }

}
