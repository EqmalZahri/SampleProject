namespace Rbac_IctJohor.Models.UsersRequest
{
    public class RequestUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Guid AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string Title { get; set; }
        public List<string> RoleNames { get; set; }
        public bool IsDisable { get; set; }
    }
}
