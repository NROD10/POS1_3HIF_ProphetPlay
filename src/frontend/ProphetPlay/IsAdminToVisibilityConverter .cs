using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProphetPlay
{
    /// <summary>
    /// Zeigt ein Element nur dann an wenn die übergebene Rolle Admin ist
    /// </summary>
    public class IsAdminToVisibilityConverter : IValueConverter
    {
        // value: AktuelleRolle (string)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string rolle)
            {
                LoggerService.Logger.Information("IsAdminToVisibilityConverter.Convert aufgerufen mit Rolle: {Rolle}", rolle);

                if (rolle.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    LoggerService.Logger.Information("Rolle ist Admin: Sichtbarkeit = Visible");
                    return Visibility.Visible;
                }

                LoggerService.Logger.Information("Rolle ist nicht Admin: Sichtbarkeit = Collapsed");
            }
            else
            {
                LoggerService.Logger.Warning("IsAdminToVisibilityConverter.Convert: value ist kein string");
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
