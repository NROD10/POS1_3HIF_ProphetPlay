using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProphetPlay
{
    public class NewsArticle
    {
        // Diese Zeile muss drin stehen, damit der Delete-Button weiß, welche News-ID
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}