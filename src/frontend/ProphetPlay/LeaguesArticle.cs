using System;

namespace ProphetPlay
{
    /// <summary>
    /// Für eine vereinfachte Datenstruktur für eine Liga z. B. für die Anzeige in der Benutzeroberfläche
    /// </summary>
    public class LeaguesArticle
    {
        /// <summary>
        /// Eindeutige ID der Liga
        /// </summary>
        public int LeagueId { get; set; }

        /// <summary>
        /// Name der Liga
        /// </summary>
        public string LeagueName { get; set; }

        /// <summary>
        /// URL zum Logo der Liga
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Name des Landes in dem die Liga stattfindet
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Aktuelle oder zuletzt bekannte Saison
        /// </summary>
        public int Season { get; set; }
    }
}
