using Newtonsoft.Json;

namespace Rbac_IctJohor.Models.Dto
{
    public class TagCreationRequest
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
