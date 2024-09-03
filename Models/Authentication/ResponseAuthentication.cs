namespace Rbac_IctJohor.Models.Authentication
{
    public class ResponseAuthentication
    {
            public string AccessToken { get; set; }
            public int ExpireInSeconds { get; set; }
            public UserMetadata User { get; set; }

     }
        public class UserMetadata
        {
            public string Id { get; set; }
            public string Username { get; set; }

            //public string Role { get; set; }
            public List<string> Roles { get; set; }

            //public string JobTitle { get; set; }
            public string Email { get; set; }
            public string TenantName { get; set; }
            public string TenantCode { get; set; }
            public string AgencyName { get; set; }
            public string AgencyCode { get; set; }

            public string AgencyId { get; set; }
    }
}
