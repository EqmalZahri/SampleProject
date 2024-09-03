using System.ComponentModel.DataAnnotations;

namespace Rbac_IctJohor.Models.Dto
{
    public class ChangePasswordById
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Password Do Not Match")]
        public string ConfirmNewPassword { get; set; }
    }
}
