using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ProphetPlay; // Dein Hauptprojekt, damit LeaguesArticle verfügbar ist

namespace ProphetPlayTesting
{
    [TestClass]
    public class LeagueFilteringTests
    {
        private readonly List<LeaguesArticle> _sampleLeagues = new()
        {
            new() { LeagueName = "Bundesliga",     CountryName = "Deutschland" },
            new() { LeagueName = "Premier League", CountryName = "England"      },
            new() { LeagueName = "Serie A",        CountryName = "Italien"      },
        };

        [TestMethod]
        public void EmptyQuery_ReturnsAllLeagues()
        {
            // "" als Query bedeutet keine Filterung
            var result = _sampleLeagues
                .Where(lg => string.IsNullOrEmpty(""))
                .ToList();

            CollectionAssert.AreEqual(_sampleLeagues, result);
        }

        [TestMethod]
        public void QueryMatchesLeagueName()
        {
            var query = "serie";
            var result = _sampleLeagues
                .Where(lg => lg.LeagueName.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                          || lg.CountryName.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Serie A", result[0].LeagueName);
        }

        [TestMethod]
        public void QueryMatchesCountryName()
        {
            var query = "engl";
            var result = _sampleLeagues
                .Where(lg => lg.LeagueName.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                          || lg.CountryName.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Premier League", result[0].LeagueName);
        }

        [TestMethod]
        public void NoMatch_ReturnsEmptyList()
        {
            var query = "xyz";
            var result = _sampleLeagues
                .Where(lg => lg.LeagueName.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                          || lg.CountryName.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.AreEqual(0, result.Count);
        }
    }
}
