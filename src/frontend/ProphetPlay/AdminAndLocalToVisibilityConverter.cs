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
            LoggerService.Logger.Information("AdminAndLocalToVisibilityConverter.Convert aufgerufen.");

            // values[0] = AktuelleRolle, values[1] = Id
            if (values.Length < 2)
            {
                LoggerService.Logger.Warning("Nicht genügend Werte übergeben an Converter.");
                return Visibility.Collapsed;
            }

            var rolle = values[0] as string;
            LoggerService.Logger.Debug("Rolle: {Rolle}", rolle);

            if (!string.Equals(rolle, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                LoggerService.Logger.Information("Benutzer ist kein Admin.");
                return Visibility.Collapsed;
            }

            // ID kann null, int oder long sein
            if (values[1] is int idInt && idInt > 0)
            {
                LoggerService.Logger.Debug("Sichtbar für Admin mit int-Id: {Id}", idInt);
                return Visibility.Visible;
            }
            if (values[1] is long idLong && idLong > 0)
            {
                LoggerService.Logger.Debug("Sichtbar für Admin mit long-Id: {Id}", idLong);
                return Visibility.Visible;
            }

            LoggerService.Logger.Information("ID nicht gültig oder kleiner gleich 0.");
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            LoggerService.Logger.Warning("ConvertBack ist nicht unterstützt und wurde aufgerufen.");
            throw new NotSupportedException();
        }
    }
}
