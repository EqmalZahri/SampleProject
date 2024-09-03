namespace Rbac_IctJohor.Models
{
    public class Agency
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();


        public string TagId { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }

        //public string AgencyGroup { get; set; }
        public List<string> AgencyGroup { get; set; } = new List<string>();

        public string Address { get; set; }
        public string AgencyContactNo { get; set; }
    }

}
