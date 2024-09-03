using System.ComponentModel.DataAnnotations;

namespace Rbac_IctJohor.Models.Authentication
{
    public class RequestAuthentication
    {
        [Required]
        //public string Username { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
