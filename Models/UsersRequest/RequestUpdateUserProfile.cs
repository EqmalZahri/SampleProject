using System.ComponentModel.DataAnnotations;

namespace Rbac_IctJohor.Models.UsersRequest
{
    public class RequestUpdateUserProfile
    {
        //[Required]
        //public string IdentificationNumber { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Title { get; set; }
        [Required]
        public Guid AgencyId { get; set; }

    }
}
