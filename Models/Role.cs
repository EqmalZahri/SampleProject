using Microsoft.AspNetCore.Identity;
using Rbac_IctJohor.Models;

namespace Rbac_IctJohor.Models
{
    public class Role : IdentityRole<Guid>
    {
        public Role(string roleName)
        {
            Name = roleName;
            NormalizedName = roleName.ToUpper();
        }

        public Role()
        {
            
        }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }


}
