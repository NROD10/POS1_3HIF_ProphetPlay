using Newtonsoft.Json;

namespace ProphetPlay
{
    /// <summary>
    /// Modellklasse zum Erstellen eines neuen News-Artikels für die API.
    /// </summary>
    public class NewsArticleCreate
    {
        /// <summary>
        /// Titel der News.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Beschreibung oder Inhalt der News.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Link zur Quelle oder weiterführenden Informationen.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
