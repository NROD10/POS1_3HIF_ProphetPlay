using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProphetPlay
{
    public class LiveMatchesApiResponse
    {
        [JsonProperty("response")]
        public List<LiveMatchResponse> Response { get; set; }
    }

    public class LiveMatchResponse
    {
        [JsonProperty("league")]
        public League League { get; set; }

        [JsonProperty("teams")]
        public Teams Teams { get; set; }

        [JsonProperty("goals")]
        public Goals Goals { get; set; }

        [JsonProperty("fixture")]
        public Fixture Fixture { get; set; }

        // ✅ Neue Hilfseigenschaften für die Anzeige (optional, brechen nichts)

        public string TeamsString => $"{Teams?.Home?.Name} vs {Teams?.Away?.Name}";

        public string StartTime
        {
            get
            {
                if (DateTime.TryParse(Fixture?.Date, out DateTime dt))
                    return dt.ToLocalTime().ToString("HH:mm");
                return "-";
            }
        }

        public string Status => Fixture?.Status?.Short;
    }

    public class Fixture
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("status")]
        public FixtureStatus Status { get; set; }
    }

    public class FixtureStatus
    {
        [JsonProperty("long")]
        public string Long { get; set; }

        [JsonProperty("short")]
        public string Short { get; set; }

        [JsonProperty("elapsed")]
        public int? Elapsed { get; set; }
    }

    public class League
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class Teams
    {
        [JsonProperty("home")]
        public Team Home { get; set; }

        [JsonProperty("away")]
        public Team Away { get; set; }
    }

    public class Team
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Goals
    {
        [JsonProperty("home")]
        public int? Home { get; set; }

        [JsonProperty("away")]
        public int? Away { get; set; }
    }
}
