using Newtonsoft.Json;

namespace ProphetPlay
{
    public class NewsArticleCreate
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
