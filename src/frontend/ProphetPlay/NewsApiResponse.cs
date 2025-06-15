using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProphetPlay
{
    public class NewsApiResponse
    {
        [JsonProperty("articles")]
        public List<NewsArticle> Articles { get; set; }
    }
}
