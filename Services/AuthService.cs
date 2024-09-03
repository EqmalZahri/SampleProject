using Rbac_IctJohor.Extensions;
using Rbac_IctJohor.common.Constants;
using Rbac_IctJohor.Models.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Rbac_IctJohor.Models;

namespace Rbac_IctJohor.Services
{
    public interface IAuthService
    {
        ResponseAuthentication CreateAccessToken(string userId, string username,
            IEnumerable<string> userRoles, string email, string tenantName, string tenantCode, string agencyName, string agencyCode, string agencyId);
    }
    public class AuthService : IAuthService
    {
        private readonly TokenAuthConfiguration _tokenConfig;

        public AuthService(TokenAuthConfiguration tokenConfig)
        {
            _tokenConfig = tokenConfig;
        }

        public ResponseAuthentication CreateAccessToken(string userId, string username,
            IEnumerable<string> userRoles, string email, string tenantName, string tenantCode, string agencyName, string agencyCode, string agencyId)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                //new Claim(ClaimConst.JOBTITLE, String.IsNullOrWhiteSpace(jobTitle) ? "": jobTitle),
                new Claim ("tenantName", tenantName),
                new Claim("tenantCode", tenantCode),
                new Claim("agencyName", agencyName),
                new Claim("agencyCode", agencyCode),
                new Claim("agencyId", agencyId),
            };

            //if (userRoles.Any())
            //{
            //    var roles = string.Join(",", userRoles);
            //    claims.Add(new Claim(ClaimTypes.Role, roles));
            //}

            foreach (var role in userRoles)
            { 
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var now = DateTime.UtcNow;
            var timeSpanPeriod = _tokenConfig.Expiration;
            DateTime period = now.Add(_tokenConfig.Expiration);

            var jwtSecurityToken = new JwtSecurityToken(
              issuer: _tokenConfig.Issuer,
              audience: _tokenConfig.Audience,
              claims: claims,
              notBefore: now,
              expires: period,
              signingCredentials: _tokenConfig.SigningCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return new ResponseAuthentication
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)timeSpanPeriod.TotalSeconds,
                User = new UserMetadata
                {
                    Id = userId,
                    Username = username,
                    Email = email,
                    //Role = userRoles.FirstOrDefault(),
                    Roles = userRoles.ToList(),
                    TenantName = tenantName,
                    TenantCode = tenantCode,
                    AgencyName = agencyName,
                    AgencyCode = agencyCode,
                    AgencyId = agencyId,

                }
            };

        }
    }
}
