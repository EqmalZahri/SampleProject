namespace Rbac_IctJohor.Models.Dto
{
    public class ReqCreateUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public Guid JobTitleId { get; set; }
        public string Password { get; set; }
        public Guid AgencyId { get; set; }
        public string Title { get; set; }
        public List<string> RoleNames { get; set; } = new List<string>();
    }

}
