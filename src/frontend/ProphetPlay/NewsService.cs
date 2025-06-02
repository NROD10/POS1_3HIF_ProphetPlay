using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    public static class NewsService
    {
        // Prompt: wie hole ich die Newsdaten aus dieser Api heraus
        private static readonly string apiKey = "93cc6b28049747b9848ddccc5797890c";
        private static readonly string apiUrl = $"https://newsapi.org/v2/everything?q=fussball&language=de&sortBy=publishedAt&apiKey=93cc6b28049747b9848ddccc5797890c";


        public static async Task<List<NewsArticle>> GetFootballNewsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(apiUrl);
                NewsApiResponse result = JsonConvert.DeserializeObject<NewsApiResponse>(response);
                return result.Articles;
            }
        }
    }
}
