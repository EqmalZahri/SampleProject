namespace Rbac_IctJohor.Models.UsersRequest
{
    //public class RequestCreateUser : RequestAddUser
    public class RequestCreateUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> RoleNames { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //public Guid JobTitleId { get; set; }


    }
}
