using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Serialization;


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
        public static async Task<List<NewsArticle>> GetAllNewsAsync()
        {
            using var client = new HttpClient();
            var json = await client.GetStringAsync("http://localhost:8080/api/news");
            return JsonConvert.DeserializeObject<List<NewsArticle>>(json);
        }

        public static async Task<bool> CreateNewsAsync(NewsArticleCreate dto, string requester)
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var json = JsonConvert.SerializeObject(dto, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync($"api/news?requester={requester}", content);
            return resp.IsSuccessStatusCode;
        }


        // NewsService.cs
        public static async Task<bool> DeleteNewsAsync(int id, string requester)
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
            var resp = await client.DeleteAsync($"api/news?id={id}&requester={Uri.EscapeDataString(requester)}");
            return resp.IsSuccessStatusCode;
        }


    }

}
