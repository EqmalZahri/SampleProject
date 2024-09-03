using System.ComponentModel.DataAnnotations;

namespace Rbac_IctJohor.Models.UsersRequest
{
    public class RequestChangePassword
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password Do Not Match")]
        public string ConfirmNewPassword { get; set; }
    }
}
