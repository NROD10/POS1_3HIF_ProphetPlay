using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProphetPlay
{
    /// <summary>
    /// Repräsentiert die Antwortstruktur für Ereignisse eines Fixtures bzw. Spiels von der API
    /// </summary>
    public class FixtureEventsApiResponse
    {
        /// <summary>
        /// Liste der Spielereignisse, z. B. Tore, Karten, Auswechslungen
        /// </summary>
        [JsonProperty("response")]
        public List<FixtureEvent> Response { get; set; }
    }

    /// <summary>
    /// Zeigt ein einzelnes Spielereignis
    /// </summary>
    public class FixtureEvent
    {
        /// <summary>
        /// Zeitinformation des Ereignisses
        /// </summary>
        [JsonProperty("time")]
        public TimeInfo Time { get; set; }

        /// <summary>
        /// Das Team, das am Ereignis beteiligt ist
        /// </summary>
        [JsonProperty("team")]
        public TeamInfo Team { get; set; }

        /// <summary>
        /// Der Spieler der am Ereignis beteiligt ist
        /// </summary>
        [JsonProperty("player")]
        public PlayerInfo Player { get; set; }

        /// <summary>
        /// Detailangabe zum Ereignis
        /// </summary>
        [JsonProperty("detail")]
        public string Detail { get; set; }

        /// <summary>
        /// Typ des Ereignisses wie Goal oder Card
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gibt den Teamnamen zurück als Hilfs-Property zur Datenbindung
        /// </summary>
        public string TeamName => Team?.Name ?? "";
    }

    /// <summary>
    /// Zeitangabe eines Ereignisses
    /// </summary>
    public class TimeInfo
    {
        /// <summary>
        /// Gespielte Minuten zum Zeitpunkt des Ereignisses
        /// </summary>
        [JsonProperty("elapsed")]
        public int Elapsed { get; set; }
    }

    /// <summary>
    /// Zeigt Teamname bei einem Ereignis
    /// </summary>
    public class TeamInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Zeigt Spielernamen bei einem Ereignis
    /// </summary>
    public class PlayerInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}