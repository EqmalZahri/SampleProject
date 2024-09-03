using System.ComponentModel.DataAnnotations;

namespace Rbac_IctJohor.Models
{
    public class BaseClass
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
