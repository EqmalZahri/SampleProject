using Newtonsoft.Json;

namespace Rbac_IctJohor.Models.Dto
{
    public class TagCreationResponse
    {
        [JsonProperty("tag_id")]
        public string TagId { get; set; }
    }
}
