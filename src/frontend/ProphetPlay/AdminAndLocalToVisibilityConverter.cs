// Datei: AdminAndLocalToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProphetPlay
{
    /// <summary>
    /// Sichtbarkeit nur für Admins UND eigene News (Id &gt; 0).
    /// </summary>
    public class AdminAndLocalToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = AktuelleRolle, values[1] = Id
            if (values.Length < 2)
                return Visibility.Collapsed;

            var rolle = values[0] as string;
            if (!string.Equals(rolle, "Admin", StringComparison.OrdinalIgnoreCase))
                return Visibility.Collapsed;

            // ID kann null, int oder long sein
            if (values[1] is int idInt && idInt > 0)
                return Visibility.Visible;
            if (values[1] is long idLong && idLong > 0)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
