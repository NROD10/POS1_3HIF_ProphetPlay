using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProphetPlay
{
    /// <summary>
    /// Zeigt ein Element nur dann an, wenn die übergebene Rolle "Admin" ist.
    /// </summary>
    public class IsAdminToVisibilityConverter : IValueConverter
    {
        // value: AktuelleRolle (string)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string rolle && rolle.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
