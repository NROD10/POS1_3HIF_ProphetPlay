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

        /// <summary>
        /// Holt aktuelle Fußball-News von der externen News API
        /// </summary>
        /// <returns>Liste von News-Artikeln</returns>
        public static async Task<List<NewsArticle>> GetFootballNewsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "ProphetPlayClient/1.0");

                try
                {
                    var warmup = await client.GetAsync(apiUrl);
                    if (!warmup.IsSuccessStatusCode)
                    {
                        throw new Exception($"News API nicht erreichbar: {warmup.StatusCode}");
                    }

                    string json = await warmup.Content.ReadAsStringAsync();
                    NewsApiResponse result = JsonConvert.DeserializeObject<NewsApiResponse>(json);

                    if (result?.Articles == null)
                        throw new Exception("Ungültiges News-Format");

                    LoggerService.Logger.Information("Fußball-News erfolgreich geladen.");
                    return result.Articles;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("News API Fehler: " + ex.Message);
                    LoggerService.Logger.Error(ex, "Fehler beim Laden der Fußball-News.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Holt alle News aus der lokalen API
        /// </summary>
        /// <returns>Liste von News-Artikeln</returns>
        public static async Task<List<NewsArticle>> GetAllNewsAsync()
        {
            using var client = new HttpClient();
            try
            {
                var json = await client.GetStringAsync("http://localhost:8080/api/news");
                LoggerService.Logger.Information("Alle News von lokaler API geladen.");
                return JsonConvert.DeserializeObject<List<NewsArticle>>(json);
            }
            catch (Exception ex)
            {
                LoggerService.Logger.Error(ex, "Fehler beim Laden aller News von lokaler API.");
                throw;
            }
        }

        /// <summary>
        /// Erstellt einen neuen News-Artikel über die API
        /// </summary>
        /// <param name="dto">News-Daten</param>
        /// <param name="requester">Benutzername des Anfragenden</param>
        /// <returns>True wenn erfolgreich</returns>
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

            if (resp.IsSuccessStatusCode)
            {
                LoggerService.Logger.Information("News erstellt von {User}", requester);
                return true;
            }
            else
            {
                LoggerService.Logger.Warning("News-Erstellung von {User} fehlgeschlagen: {StatusCode}", requester, resp.StatusCode);
                return false;
            }
        }

        /// <summary>
        /// Löscht einen News-Artikel anhand der ID
        /// </summary>
        /// <param name="id">ID des zu löschenden News-Artikels</param>
        /// <param name="requester">Benutzername des Anfragenden</param>
        /// <returns>True wenn erfolgreich gelöscht</returns>
        /// 
            // Prompt: "DeleteNewsAsync: DELETE /api/news?id=&requester= und gib true/false zurück"
        public static async Task<bool> DeleteNewsAsync(int id, string requester)
        {
            using var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
            var resp = await client.DeleteAsync($"api/news?id={id}&requester={Uri.EscapeDataString(requester)}");

            if (resp.IsSuccessStatusCode)
            {
                LoggerService.Logger.Information("News mit ID {Id} gelöscht von {User}", id, requester);
                return true;
            }
            else
            {
                LoggerService.Logger.Warning("Löschen der News mit ID {Id} durch {User} fehlgeschlagen: {StatusCode}", id, requester, resp.StatusCode);
                return false;
            }
        }
    }
}
