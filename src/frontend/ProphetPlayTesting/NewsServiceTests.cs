using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProphetPlay;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ProphetPlayTesting
{
    [TestClass]
    public class NewsServiceTests
    {

        // F�r Unitests allgemein gr��teils Chatgpt verwendet

        [TestMethod]
        public async Task GetAllNewsAsync_ReturnsNonNullList()
        {
            var all = await NewsService.GetAllNewsAsync();
            Assert.IsNotNull(all, "GetAllNewsAsync darf keine null zur�ckgeben");
            Assert.IsInstanceOfType(all, typeof(List<NewsArticle>));
        }

        [TestMethod]
        public async Task GetAllNewsAsync_ContainsOnlyValidItems()
        {
            var all = await NewsService.GetAllNewsAsync();
            Assert.IsTrue(all.All(n => !string.IsNullOrWhiteSpace(n.Title)),
                          "Jeder News-Item muss einen Title haben");
        }

        [TestMethod]
        public async Task DeleteNewsAsync_InvalidId_ReturnsFalse()
        {
            bool ok = await NewsService.DeleteNewsAsync(-1, "admin");
            Assert.IsFalse(ok, "DeleteNewsAsync muss bei ung�ltiger ID false zur�ckgeben");
        }

        [TestMethod]
        public async Task DeleteNewsAsync_NonexistentId_ReturnsFalse()
        {
            bool ok = await NewsService.DeleteNewsAsync(999999, "admin");
            Assert.IsFalse(ok, "DeleteNewsAsync muss false liefern, wenn die News nicht existiert");
        }
    }
}
