using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class LeagueInfo
    {
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

}
