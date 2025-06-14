using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProphetPlay
{
    public static class ApiFootballService
    {
        private static readonly HttpClient _client;
        private const string ApiKey = "47b2ec2efa506d41c568e5858f9540ea";

        static ApiFootballService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("x-apisports-key", ApiKey);
        }

        // vorhandene Methode...
        public static async Task<List<LeaguesArticle>> GetLeaguesAsync()
        {
            var json = await _client.GetStringAsync("https://v3.football.api-sports.io/leagues");
            var result = JsonConvert.DeserializeObject<LeaguesApiResponse>(json);

            return result.Response.Select(x => new LeaguesArticle
            {
                LeagueId = x.League.Id,
                LeagueName = x.League.Name,
                LogoUrl = x.League.Logo,
                CountryName = x.Country.Name,
                Season = x.Seasons?.FirstOrDefault(s => s.Current)?.Year
                          ?? x.Seasons?.Max(s => s.Year)
                          ?? DateTime.UtcNow.Year
            }).ToList();
        }

        /// <summary>
        /// Holt die nächsten 'next' Spiele einer Liga/Saison (Live oder anstehend).
        /// </summary>
        public static async Task<List<LiveMatchResponse>> GetUpcomingMatchesAsync(int leagueId, int season, int next = 10)
        {
            string url = $"https://v3.football.api-sports.io/fixtures" +
                         $"?league={leagueId}" +
                         $"&season={season}" +
                         $"&next={next}";
            var json = await _client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(json);
            return result.Response ?? new List<LiveMatchResponse>();
        }

        /// <summary>
        /// Holt die letzten 'last' Spiele einer Liga/Saison (Ergebnisse).
        /// </summary>
        public static async Task<List<LiveMatchResponse>> GetPastMatchesAsync(int leagueId, int season, int last = 10)
        {
            string url = $"https://v3.football.api-sports.io/fixtures" +
                         $"?league={leagueId}" +
                         $"&season={season}" +
                         $"&last={last}";
            var json = await _client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(json);
            return result.Response ?? new List<LiveMatchResponse>();
        }

        /// <summary>
        /// Fallback: Spiele in den nächsten 'days' Tagen abrufen.
        /// </summary>
        public static async Task<List<LiveMatchResponse>> GetMatchesByDateRangeAsync(int leagueId, int season, int days = 30)
        {
            var from = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");

            string url = $"https://v3.football.api-sports.io/fixtures" +
                         $"?league={leagueId}" +
                         $"&season={season}" +
                         $"&from={from}" +
                         $"&to={to}";
            var json = await _client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(json);
            return result.Response ?? new List<LiveMatchResponse>();
        }

        /// <summary>
        /// Holt alle Events (Tore, Karten, Wechsel…) zu einem Fixture
        /// </summary>
        public static async Task<List<FixtureEvent>> GetFixtureEventsAsync(int fixtureId)
        {
            string url = $"https://v3.football.api-sports.io/fixtures/events?fixture={fixtureId}";
            var json = await _client.GetStringAsync(url);
            var wrapper = JsonConvert.DeserializeObject<FixtureEventsApiResponse>(json);
            return wrapper.Response ?? new List<FixtureEvent>();
        }
    }
}
