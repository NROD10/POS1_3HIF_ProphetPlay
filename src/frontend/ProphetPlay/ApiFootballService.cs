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
    public class ApiFootballService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey = "e1070428ce875dfd9b406c1e4a1fb7ab";

        // Prompt: wie binde ich eine API in c# ein?
        public ApiFootballService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("x-apisports-key", _apiKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Live Spiele
        public async Task<JObject> GetLiveMatchesAsync()
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
        public async Task<JObject> GetMatchesByDateAsync(string date)
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

        // News
        public async Task<JObject> GetNewsAsync()
        {
            string url = "https://v3.football.api-sports.io/news";

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

        public async Task<JObject> GetLeaguesAsync()
        {
            string url = "https://v3.football.api-sports.io/leagues";

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

    }
}
