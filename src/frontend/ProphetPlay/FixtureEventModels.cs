using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProphetPlay
{
    public class FixtureEventsApiResponse
    {
        [JsonProperty("response")]
        public List<FixtureEvent> Response { get; set; }
    }

    public class FixtureEvent
    {
        [JsonProperty("time")]
        public TimeInfo Time { get; set; }

        [JsonProperty("team")]
        public TeamInfo Team { get; set; }

        [JsonProperty("player")]
        public PlayerInfo Player { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }    // z.B. "Regular Goal", "Yellow Card"

        [JsonProperty("type")]
        public string Type { get; set; }      // "Goal", "Card", …

        // ⬇︎ Hilfs‐Property, um direkt den Teamnamen zu binden
        public string TeamName => Team?.Name ?? "";
    }

    public class TimeInfo
    {
        [JsonProperty("elapsed")]
        public int Elapsed { get; set; }      // Spielminute
    }

    public class TeamInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class PlayerInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
