using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetPlay
{
    public static class NewsService
    {
        // Deinen API-Key hier eintragen
        private static readonly string apiKey = "93cc6b28049747b9848ddccc5797890c";

        // Top-Headlines aus der Sport-Kategorie in Deutschland
        private static readonly string apiUrl =
            $"https://newsapi.org/v2/everything?q=fussball&language=de&sortBy=publishedAt&apiKey=93cc6b28049747b9848ddccc5797890c";

        public static async Task<List<NewsArticle>> GetFootballNewsAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();  // wirft, wenn Status != 2xx

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<NewsApiResponse>(json);
            return result?.Articles ?? new List<NewsArticle>();
        }
    }
}
