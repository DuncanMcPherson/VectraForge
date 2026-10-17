using System.Globalization;
using Avalonia.Data.Converters;

namespace VectraForge.Converters;

public class ParentDirectoryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value is not string str)
            return value;
        return Path.GetDirectoryName(str);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}