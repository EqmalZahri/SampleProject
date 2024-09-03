using EZHelper.EfCore.Pagination;

namespace Rbac_IctJohor.Models
{
    public class PaginationUserRequest : PaginatedModel
    {
        public string Filter { get; set; }
        public bool FilterDisable { get; set; }
    }
}
