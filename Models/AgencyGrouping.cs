namespace Rbac_IctJohor.Models
{
    public class AgencyGrouping
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Adding Id property for primary key
        public string GroupName { get; set; }
        public string GroupDesc { get; set; }
    }
}
