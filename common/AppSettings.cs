#nullable disable
namespace Rbac_IctJohor.common
{
    public class AppSettings
    {
        //public string Secret { get; set; }
        public string CORS {  get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public Authentication Authentication { get; set; }

    }

    public class ConnectionStrings
    {
        public string AppConnection { get; set; }
    }
    public class Authentication
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenLifeTime { get; set; }
    }
}
