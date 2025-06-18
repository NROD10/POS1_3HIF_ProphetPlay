using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProphetPlay
{
    /// <summary>
    /// Repräsentiert die gesamte API-Antwort für verfügbare Ligen
    /// </summary>
    public class LeaguesApiResponse
    {
        /// <summary>
        /// Liste der Liga-Antworten mit Informationen zu Ligen, Ländern und Saisons
        /// </summary>
        [JsonProperty("response")]
        public List<LeagueResponse> Response { get; set; }
    }

    /// <summary>
    /// Ist die Liga-Antwort mit Liga-, Land- und Saisoninformationen.
    /// </summary>
    public class LeagueResponse
    {
        /// <summary>
        /// Informationen zur Liga wie Name, ID, Logo
        /// </summary>
        [JsonProperty("league")]
        public LeagueInfo League { get; set; }

        /// <summary>
        /// Informationen zum Land der Liga
        /// </summary>
        [JsonProperty("country")]
        public CountryInfo Country { get; set; }

        /// <summary>
        /// Liste der verfügbaren Saisons für die Liga
        /// </summary>
        [JsonProperty("seasons")]
        public List<SeasonInfo> Seasons { get; set; }
    }

    /// <summary>
    /// Informationen über eine Liga wie Id, Name, Logo
    /// </summary>
    public class LeagueInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }

    /// <summary>
    /// Name des Landes wo sich die Liga befindet
    /// </summary>
    public class CountryInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Zeigt das Jahr der Saison einer Liga und ob die Saison aktuell ist
    /// </summary>
    public class SeasonInfo
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("current")]
        public bool Current { get; set; }
    }
}