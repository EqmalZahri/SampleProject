using Microsoft.AspNetCore.Identity;
using Rbac_IctJohor.Models;

namespace Rbac_IctJohor.Models
{
    public class User : IdentityUser<Guid>
    {
        public string Fullname { get; set; }
        public string Title { get; set; }
        public bool ChangePassword { get; set; }
        public Guid? JobTitleId { get; set; }
        public bool IsDisable { get; set; }
        public Guid AgencyId { get; set; }
        public Agency Agency { get; set; }
        public bool IsPersonInCharge { get; set; }

    }
}
