using System.Security;

namespace Rbac_IctJohor.Models
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }

        // Add composite primary key definition
        public override int GetHashCode()
        {
            return HashCode.Combine(RoleId, PermissionId);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not RolePermission)
                return false;

            var other = (RolePermission)obj;
            return RoleId == other.RoleId && PermissionId == other.PermissionId;
        }
    }
}
