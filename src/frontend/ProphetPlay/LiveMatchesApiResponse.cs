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

        // Anzeige: "Heim vs Auswärts"
        public string TeamsString =>
            $"{Teams?.Home?.Name} vs {Teams?.Away?.Name}";

        // Datum + Uhrzeit im Format "dd.MM.yyyy HH:mm"
        public string MatchDateTime
        {
            get
            {
                if (DateTime.TryParse(Fixture?.Date, out DateTime dt))
                    return dt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
                return "-";
            }
        }

        // Spielstand: echtes Ergebnis, wenn live oder vorbei; sonst "0:0 (bevorstehend)"
        public string DisplayScore
        {
            get
            {
                var status = Fixture?.Status?.Short;
                if (status == "NS" || status == "TBD")
                    return "0:0 (bevorstehend)";
                return $"{Goals?.Home ?? 0}:{Goals?.Away ?? 0}";
            }
        }

        // Kurz-Status (LIVE, FT, NS, etc.)
        public string Status =>
            Fixture?.Status?.Short ?? "";

        // Wichtige ID für Detail-Abfrage
        public int FixtureId => Fixture?.Id ?? 0;
    }

    public class League
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
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
        [JsonProperty("short")]
        public string Short { get; set; }
    }
}
