using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;                    
using ProphetPlay;                        

namespace ProphetPlayTesting
{
    [TestClass]
    public class VisibilityConverterTests
    {


        // Für Unitests allgemein größteils Chatgpt verwendet

        private readonly IsAdminToVisibilityConverter _isAdmin = new();
        private readonly AdminAndLocalToVisibilityConverter _adminAndLocal = new();

        [DataTestMethod]
        [DataRow("Admin", Visibility.Visible)]
        [DataRow("admin", Visibility.Visible)]
        [DataRow("User", Visibility.Collapsed)]
        public void IsAdminToVisibilityConverter_Works(string role, Visibility expected)
        {
            var actual = (Visibility)_isAdmin.Convert(role, typeof(Visibility), null, null);
            Assert.AreEqual(expected, actual);
        }
        [DataTestMethod]
        [DataRow("Admin", 1, Visibility.Visible)]
        [DataRow("Admin", 0, Visibility.Collapsed)]
        [DataRow("User", 5, Visibility.Collapsed)]
        public void AdminAndLocalToVisibilityConverter_Works(string role, object id, Visibility expected)
        {
            var actual = (Visibility)_adminAndLocal.Convert(
                new object[] { role, id },
                typeof(Visibility),
                null,
                null);
            Assert.AreEqual(expected, actual);
        }
    }
}
