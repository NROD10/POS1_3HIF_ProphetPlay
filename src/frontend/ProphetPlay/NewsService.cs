using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ProphetPlay
{
    public static class NewsService
    {
        static readonly string apiKey = "93cc6b28049747b9848ddccc5797890c";
        private static readonly string apiUrl = $"https://newsapi.org/v2/everything?q=fussball&language=de&sortBy=publishedAt&apiKey={apiKey}";

        public static async Task<List<NewsArticle>> GetFootballNewsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // WICHTIG: User-Agent setzen! Sonst blocken viele APIs
                client.DefaultRequestHeaders.Add("User-Agent", "ProphetPlayClient/1.0");

                try
                {
                    // Optional: Ersten Warm-up-Ping senden
                    var warmup = await client.GetAsync(apiUrl);
                    if (!warmup.IsSuccessStatusCode)
                    {
                        throw new Exception($"News API nicht erreichbar: {warmup.StatusCode}");
                    }

                    string json = await warmup.Content.ReadAsStringAsync();
                    NewsApiResponse result = JsonConvert.DeserializeObject<NewsApiResponse>(json);

                    if (result?.Articles == null)
                        throw new Exception("Ungültiges News-Format");

                    return result.Articles;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("News API Fehler: " + ex.Message);
                    throw; // Weiterwerfen an MainWindow
                }
            }
        }
    }

}
