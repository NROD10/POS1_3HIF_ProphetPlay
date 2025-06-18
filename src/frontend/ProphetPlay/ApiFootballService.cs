using Newtonsoft.Json;
using ProphetPlay;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Stellt Methoden zum Abrufen von Fußball-Daten über die API-Football-REST-API bereit
/// </summary>
public static class ApiFootballService
{
    private static readonly HttpClient _client;
    private const string ApiKey = "47b2ec2efa506d41c568e5858f9540ea";

    static ApiFootballService()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("x-apisports-key", ApiKey);
        LoggerService.Logger.Information("HttpClient mit API-Schlüssel initialisiert.");
    }

    public static async Task<List<LeaguesArticle>> GetLeaguesAsync()
    {
        LoggerService.Logger.Information("GetLeaguesAsync aufgerufen...");
        var json = await _client.GetStringAsync("https://v3.football.api-sports.io/leagues");
        LoggerService.Logger.Debug("Empfangenes JSON für Ligen: {0}", json);
        var result = JsonConvert.DeserializeObject<LeaguesApiResponse>(json);

        List<LeaguesArticle> articles = new List<LeaguesArticle>();

        foreach (var x in result.Response)
        {
            LeaguesArticle article = new LeaguesArticle();
            article.LeagueId = x.League.Id;
            article.LeagueName = x.League.Name;
            article.LogoUrl = x.League.Logo;
            article.CountryName = x.Country.Name;

            // Prompt (ChatGPT): "Wähle aktuelle Saison oder die höchste vorhandene."
            int seasonYear = DateTime.UtcNow.Year;

            if (x.Seasons != null)
            {
                foreach (var s in x.Seasons)
                {
                    if (s.Current)
                    {
                        seasonYear = s.Year;
                        break;
                    }
                }

                if (seasonYear == DateTime.UtcNow.Year)
                {
                    int max = 0;
                    foreach (var s in x.Seasons)
                    {
                        if (s.Year > max)
                        {
                            max = s.Year;
                        }
                    }

                    if (max > 0)
                    {
                        seasonYear = max;
                    }
                }
            }

            article.Season = seasonYear;

            articles.Add(article);
        }

        LoggerService.Logger.Information("{0} Ligen zurückgegeben.", articles.Count);
        return articles;
    }

    public static async Task<List<LiveMatchResponse>> GetUpcomingMatchesAsync(int leagueId, int season, int next = 10)
    {
        // Prompt (ChatGPT): "Logge URL und JSON bei kommenden Spielen."
        string url = $"https://v3.football.api-sports.io/fixtures" +
                     $"?league={leagueId}" +
                     $"&season={season}" +
                     $"&next={next}";
        LoggerService.Logger.Information("GetUpcomingMatchesAsync mit URL aufgerufen: {0}", url);
        var json = await _client.GetStringAsync(url);
        LoggerService.Logger.Debug("Empfangenes JSON für kommende Spiele: {0}", json);
        var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(json);

        if (result.Response != null)
        {
            LoggerService.Logger.Information("{0} kommende Spiele zurückgegeben.", result.Response.Count);
            return result.Response;
        }
        else
        {
            LoggerService.Logger.Warning("Keine kommenden Spiele gefunden.");
            return new List<LiveMatchResponse>();
        }
    }

    public static async Task<List<LiveMatchResponse>> GetPastMatchesAsync(int leagueId, int season, int last = 10)
    {

        // Prompt: Hol mir die vergangenen Live Matches aus der API
        string url = $"https://v3.football.api-sports.io/fixtures" +
                     $"?league={leagueId}" +
                     $"&season={season}" +
                     $"&last={last}";
        LoggerService.Logger.Information("GetPastMatchesAsync mit URL aufgerufen: {0}", url);
        var json = await _client.GetStringAsync(url);
        LoggerService.Logger.Debug("Empfangenes JSON für vergangene Spiele: {0}", json);
        var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(json);

        if (result.Response != null)
        {
            LoggerService.Logger.Information("{0} vergangene Spiele zurückgegeben.", result.Response.Count);
            return result.Response;
        }
        else
        {
            LoggerService.Logger.Warning("Keine vergangenen Spiele gefunden.");
            return new List<LiveMatchResponse>();
        }
    }

    public static async Task<List<LiveMatchResponse>> GetMatchesByDateRangeAsync(int leagueId, int season, int days = 30)
    {
        var from = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var to = DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");

        string url = $"https://v3.football.api-sports.io/fixtures" +
                     $"?league={leagueId}" +
                     $"&season={season}" +
                     $"&from={from}" +
                     $"&to={to}";
        LoggerService.Logger.Information("GetMatchesByDateRangeAsync mit URL aufgerufen: {0}", url);
        var json = await _client.GetStringAsync(url);
        LoggerService.Logger.Debug("Empfangenes JSON für Spiele im Datumsbereich: {0}", json);
        var result = JsonConvert.DeserializeObject<LiveMatchesApiResponse>(json);

        if (result.Response != null)
        {
            LoggerService.Logger.Information("{0} Spiele im angegebenen Zeitraum zurückgegeben.", result.Response.Count);
            return result.Response;
        }
        else
        {
            LoggerService.Logger.Warning("Keine Spiele im angegebenen Zeitraum gefunden.");
            return new List<LiveMatchResponse>();
        }
    }

    public static async Task<List<FixtureEvent>> GetFixtureEventsAsync(int fixtureId)
    {

        // Schreibe eine methode die Spielereignisse von der api abruft
        string url = $"https://v3.football.api-sports.io/fixtures/events?fixture={fixtureId}";
        LoggerService.Logger.Information("GetFixtureEventsAsync mit Spiel-ID {0} aufgerufen.", fixtureId);
        var json = await _client.GetStringAsync(url);
        LoggerService.Logger.Debug("Empfangenes JSON für Spielereignisse: {0}", json);
        var wrapper = JsonConvert.DeserializeObject<FixtureEventsApiResponse>(json);

        if (wrapper.Response != null)
        {
            LoggerService.Logger.Information("{0} Spielereignisse zurückgegeben.", wrapper.Response.Count);
            return wrapper.Response;
        }
        else
        {
            LoggerService.Logger.Warning("Keine Spielereignisse gefunden.");
            return new List<FixtureEvent>();
        }
    }
}