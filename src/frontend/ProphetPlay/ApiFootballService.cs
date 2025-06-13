using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    public static class ApiFootballService
    {
        private static readonly HttpClient _client;
        private static readonly string apiKey = "8b47ed9970bf67f6ea16c39df052a40c";

        


        public static async Task<List<LeaguesArticle>> GetLeaguesAsync()
        {
            string league_url = "https://v3.football.api-sports.io/leagues";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-apisports-key", "8b47ed9970bf67f6ea16c39df052a40c");

                string response = await client.GetStringAsync(league_url);
                LeaguesApiResponse result = JsonConvert.DeserializeObject<LeaguesApiResponse>(response);

                return result.Response.Select(x => new LeaguesArticle
                {
                    LeagueId = x.League.Id,
                    LeagueName = x.League.Name,
                    LogoUrl = x.League.Logo,
                    CountryName = x.Country.Name
                }).ToList();
            }
        }



        public static async Task<List<string>> GetLiveMatchesAsync(int leagueId)
        {
            string live_url = "https://v3.football.api-sports.io/fixtures?live=all";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-apisports-key", "8b47ed9970bf67f6ea16c39df052a40c");

                string response = await client.GetStringAsync(live_url);
                LiveMatchesApiResponse result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(response);

                return result.Response
                            .Where(match => match.League?.Id == leagueId)
                            .Select(match => $"{match.Teams.Home.Name} vs {match.Teams.Away.Name}  : {match.Goals.Home ?? 0} - {match.Goals.Away ?? 0}")
                            .ToList();


            }
        }

        // Spiel für ein bestimmtes Datum
        public static async Task<List<LiveMatchResponse>> GetMatchesByDateAsync(string date)
        {
            string url = $"https://v3.football.api-sports.io/fixtures?date={date}";
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(content);
                return result?.Response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
                return new List<LiveMatchResponse>();
            }
        }



    }
}
