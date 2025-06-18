using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;                            // für Visibility
using ProphetPlay;                               // Namespace der Converter

namespace ProphetPlayTesting
{
    [TestClass]
    public class ErrorHandlingTests
    {
        private readonly IsAdminToVisibilityConverter _isAdmin = new();
        private readonly AdminAndLocalToVisibilityConverter _admin = new();

        /// <summary>
        /// Wenn null hereinkommt, darf kein Exception fliegen, sondern es soll Collapsed zurückkommen.
        /// </summary>
        /// 


        // Für Unitests allgemein größteils Chatgpt verwendet
        [TestMethod]
        public void IsAdminToVisibilityConverter_NullInput_ReturnsCollapsed()
        {
            var actual = (Visibility)_isAdmin.Convert(
                value: null,
                targetType: typeof(Visibility),
                parameter: null,
                culture: null);

            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        /// <summary>
        /// Wenn zu wenige Werte kommen, darf kein Exception fliegen, sondern Collapsed.
        /// </summary>
        [TestMethod]
        public void AdminAndLocalToVisibilityConverter_TooFewValues_ReturnsCollapsed()
        {
            var actual = (Visibility)_admin.Convert(
                values: new object[] { "Admin" },
                targetType: typeof(Visibility),
                parameter: null,
                culture: null);

            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        /// <summary>
        /// Wenn der ID-Wert kein int/long ist, darf kein Exception fliegen, sondern Collapsed.
        /// </summary>
        [TestMethod]
        public void AdminAndLocalToVisibilityConverter_NonNumericId_ReturnsCollapsed()
        {
            var actual = (Visibility)_admin.Convert(
                values: new object[] { "Admin", "notAnId" },
                targetType: typeof(Visibility),
                parameter: null,
                culture: null);

            Assert.AreEqual(Visibility.Collapsed, actual);
        }
    }
}
