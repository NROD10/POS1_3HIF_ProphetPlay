using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    public class NewsItem
    {
        public string Title { get; set; } // Titel der Nachricht
        public string Description { get; set; } // Beschreibung der Nachricht
        public DateTime PublishedAt { get; set; } // Veröffentlichungsdatum
        public string Url { get; set; } // URL zur vollständigen Nachricht

        // Überschreibe ToString(), um den Titel in der ListBox anzuzeigen
        public override string ToString()
        {
            return Title;
        }
    }
}
