﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProphetPlay
{
    public class LeaguesApiResponse
    {
        [JsonProperty("response")]
        public List<LeagueResponse> Response { get; set; }
    }

    public class LeagueResponse
    {
        [JsonProperty("league")]
        public LeagueInfo League { get; set; }

        [JsonProperty("country")]
        public CountryInfo Country { get; set; }

        [JsonProperty("seasons")]
        public List<SeasonInfo> Seasons { get; set; }
    }

    public class LeagueInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }

    public class CountryInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    // jaja

    public class SeasonInfo
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("current")]
        public bool Current { get; set; }
    }
}
