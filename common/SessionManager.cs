using Rbac_IctJohor.common.Constants;
using System.Security.Claims;

namespace Rbac_IctJohor.common
{
    public interface ISessionManager
    {
        string UserId { get; }
        string Role { get; }
        string Username { get; }
        string JobTitle { get; }
        string Fullname { get; }
    }
    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User
        {
            get
            {
                return _httpContextAccessor?.HttpContext?.User;
            }
        }
        public string UserId
        {
            get
            {
                return User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                    ?.Value;
            }
        }
        public string Username
        {
            get
            {
                return User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)
                    ?.Value;
            }
        }

        public string Role
        {
            get
            {
                return User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)
                    ?.Value;
            }
        }

        public string Fullname
        {
            get
            {
                return User?.Claims.FirstOrDefault(x => x.Type == ClaimConst.FULLNAME)
                    ?.Value;
            }
        }

        public string JobTitle
        {
            get
            {
                return User?.Claims.FirstOrDefault(x => x.Type == ClaimConst.JOBTITLE)
                    ?.Value;
            }
        }

    }
}
