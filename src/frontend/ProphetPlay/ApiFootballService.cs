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

        public static async Task<JObject> GetLiveMatchesAsync()
        {
            string url = "https://v3.football.api-sports.io/fixtures?live=all";

            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                return JObject.Parse(content);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API-Fehler: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unerwarteter Fehler: {ex.Message}");
                return null;
            }
        }

        // Spiel für ein bestimmtes Datum
        public static async Task<JObject> GetMatchesByDateAsync(string date)
        {
            string url = $"https://v3.football.api-sports.io/fixtures?date={date}";

            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                return JObject.Parse(content);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine($"API-error: {exception.Message}");
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"unexpectet error: {exception.Message}");
                return null;
            }
        }



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
                    LeagueName = x.League.Name,
                    LogoUrl = x.League.Logo,
                    CountryName = x.Country.Name
                }).ToList();
            }
        }
    }
}
