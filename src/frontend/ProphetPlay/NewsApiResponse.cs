using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProphetPlay
{
    /// <summary>
    /// Die Antwort der News-API welche eine Liste von Nachrichtenartikeln enthält.
    /// </summary>
    public class NewsApiResponse
    {
        [JsonProperty("articles")]
        public List<NewsArticle> Articles { get; set; }

    }
}