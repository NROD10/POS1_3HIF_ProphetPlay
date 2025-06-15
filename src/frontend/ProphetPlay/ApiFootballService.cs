using Newtonsoft.Json;
using ProphetPlay;
using System.Net.Http;

public static class ApiFootballService
{
    private static readonly HttpClient _client;
    private const string ApiKey = "47b2ec2efa506d41c568e5858f9540ea";

    // HttpClient wird einmal eingerichtet und der API-Schlüssel gesetzt.
    static ApiFootballService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("x-apisports-key", ApiKey);
    }

    /// <summary>
    /// Holt alle verfügbaren Ligen (z. B. Bundesliga, Premier League…) von der API.
    /// </summary>
    public static async Task<List<LeaguesArticle>> GetLeaguesAsync()
    {
        var json = await _client.GetStringAsync("https://v3.football.api-sports.io/leagues");
        var result = JsonConvert.DeserializeObject<LeaguesApiResponse>(json);

        // Wandelt die API-Antwort in eine Liste von LeaguesArticle um (vereinfacht für dein Programm).
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
    /// Holt die nächsten Spiele einer bestimmten Liga in einer Saison (Standard: 10 Spiele).
    /// Kann auch Live-Spiele enthalten.
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
    /// Holt die letzten Spiele (mit Ergebnissen) einer Liga/Saison (Standard: 10).
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
    /// Holt alle Spiele einer Liga in einem bestimmten Zeitraum (z. B. nächsten 30 Tage).
    /// Nützlich als Fallback, falls andere Methoden keine Daten liefern.
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
    /// Holt alle Events zu einem bestimmten Spiel (z. B. Tore, Karten, Auswechslungen).
    /// </summary>
    public static async Task<List<FixtureEvent>> GetFixtureEventsAsync(int fixtureId)
    {
        string url = $"https://v3.football.api-sports.io/fixtures/events?fixture={fixtureId}";
        var json = await _client.GetStringAsync(url);
        var wrapper = JsonConvert.DeserializeObject<FixtureEventsApiResponse>(json);
        return wrapper.Response ?? new List<FixtureEvent>();
    }
}
