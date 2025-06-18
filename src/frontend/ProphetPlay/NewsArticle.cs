using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    /// <summary>
    /// Zeigt einen einzelnen Nachrichtenartikel 
    /// mit Titel, Beschreibung, Veröffentlichungsdatum, Link
    /// </summary>
    public class NewsArticle
    {
        
        public int Id { get; set; }     // Diese Zeile muss drin stehen, damit der Delete-Button weiß, welche News-ID

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Url { get; set; }


    }
}
