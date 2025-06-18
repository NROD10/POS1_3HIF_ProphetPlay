using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProphetPlay
{

    /// <summary>
    /// Die API-Antwort für eine Liste von Live- oder geplanten Spielen
    /// </summary>
    public class LiveMatchesApiResponse
    {
        [JsonProperty("response")]
        public List<LiveMatchResponse> Response { get; set; }
    }

    /// <summary>
    /// zeigt ein einzelnes Spiel mit Liga-, Team-, Ergebnis- und Zeitinformationen
    /// </summary>
    public class LiveMatchResponse
    {
        /// <summary>
        /// Die Liga in der das Spiel stattfindet
        /// </summary>
        [JsonProperty("league")]
        public League League { get; set; }

        /// <summary>
        /// Die beiden Teams die gegeneinander spielen
        /// </summary>
        [JsonProperty("teams")]
        public Teams Teams { get; set; }

        /// <summary>
        /// Die Tore des Spiels
        /// </summary>
        [JsonProperty("goals")]
        public Goals Goals { get; set; }

        /// <summary>
        /// Daten zum Spiel z. B. ID, Datum, Status
        /// </summary>
        [JsonProperty("fixture")]
        public Fixture Fixture { get; set; }

        /// <summary>
        /// Formatierter String wer gegeneinander spielt z.B. Real Madrid vs Barcelona
        /// </summary>
        public string TeamsString
        {
            get
            {
                string homeName = "";
                string awayName = "";

                if (Teams != null)
                {
                    if (Teams.Home != null)
                    {
                        homeName = Teams.Home.Name;
                    }

                    if (Teams.Away != null)
                    {
                        awayName = Teams.Away.Name;
                    }
                }

                return homeName + " vs " + awayName;
            }
        }


        /// <summary>
        /// Datum und Uhrzeit des Spiels
        /// </summary>
        public string MatchDateTime
        {
            get
            {
                try
                {
                    if (Fixture != null && Fixture.Date != null)
                    {
                        DateTime dt = DateTime.Parse(Fixture.Date);
                        dt = dt.ToLocalTime();
                        return dt.ToString("dd.MM.yyyy HH:mm");
                    }
                }
                catch (Exception ex)
                {
                    LoggerService.Logger.Error(ex, "Fehler beim Parsen des Spieldatums: {0}", Fixture?.Date);
                }
                return "-";
            }
        }

        /// <summary>
        /// Anzeige des aktuellen Spielstands oder Platzhalter bei zukünftigem Spiel
        /// </summary>
        public string DisplayScore
        {
            get
            {
                string status = "";

                if (Fixture != null && Fixture.Status != null && Fixture.Status.Short != null)
                {
                    status = Fixture.Status.Short;
                }

                if (status == "NS" || status == "TBD")
                {
                    return "0:0 (bevorstehend)";
                }

                int homeGoals = 0;
                int awayGoals = 0;

                if (Goals != null)
                {
                    if (Goals.Home != null)
                    {
                        homeGoals = (int)Goals.Home;
                    }

                    if (Goals.Away != null)
                    {
                        awayGoals = (int)Goals.Away;
                    }
                }

                return homeGoals + ":" + awayGoals;
            }
        }


        /// <summary>
        /// Zeigt der Kurzstatus des Spiels z.B. LIVE, FT, NS
        /// </summary>
        public string Status
        {
            get
            {
                if (Fixture != null && Fixture.Status != null && Fixture.Status.Short != null)
                {
                    return Fixture.Status.Short;
                }
                else
                {
                    return "";
                }
            }
        }


        /// <summary>
        /// ID des Spiels wichtig für Detailabfragen
        /// </summary>
        public int FixtureId
        {
            get
            {
                if (Fixture != null)
                {
                    return Fixture.Id;
                }
                else
                {
                    return 0;
                }
            }
        }


    }

    /// <summary>
    /// sind die grundlegende Informationen zur Liga drinnen Id und der Name
    /// </summary>
    public class League
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Enthält beide Teams eines Spiels Heim und Auswärts
    /// </summary>
    public class Teams
    {
        [JsonProperty("home")]
        public Team Home { get; set; }

        [JsonProperty("away")]
        public Team Away { get; set; }
    }

    /// <summary>
    /// Name des einzelnes Teams
    /// </summary>
    public class Team
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Enthält die Toranzahl beider Teams
    /// </summary>
    public class Goals
    {
        [JsonProperty("home")]
        public int? Home { get; set; }

        [JsonProperty("away")]
        public int? Away { get; set; }
    }

    /// <summary>
    /// Metadaten zum Spiel ID, Datum und Status
    /// </summary>
    public class Fixture
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        /// Status des Spiels ob es live oder beendet ist
        /// </summary>
        [JsonProperty("status")]
        public FixtureStatus Status { get; set; }
    }

    /// <summary>
    /// Enthält den Kurzstatus eines Spiels NS, FT, LIVE
    /// </summary>
    public class FixtureStatus
    {
        /// <summary>
        /// Kurzcode für den Spielstatus
        /// </summary>
        [JsonProperty("short")]
        public string Short { get; set; }
    }
}