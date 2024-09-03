namespace Rbac_IctJohor.Models.Dto
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        //public string RoleName { get; set; }
        public string Email { get; set; }
        public List<string> RoleNames { get; set; }
        
    }

}
