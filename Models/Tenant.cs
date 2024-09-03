namespace Rbac_IctJohor.Models
{
    public class Tenant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenantName { get; set; }
        public string TenantCode { get; set; }
        public ICollection<Agency> Agencies { get; set; } = new List<Agency>();
    }

}
