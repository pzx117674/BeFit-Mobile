using System.Globalization;

namespace BeFit.Mobile.Converters;

/// <summary>
/// Konwerter sprawdzający czy string nie jest pusty (używany do wyświetlania błędów walidacji)
/// </summary>
public class StringNotEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
