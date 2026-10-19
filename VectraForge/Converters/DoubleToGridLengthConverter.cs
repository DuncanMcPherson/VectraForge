using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace VectraForge.Converters;

public class DoubleToGridLengthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new GridLength(value is double d && double.IsFinite(d) ? Math.Max(0, d) : 0, GridUnitType.Pixel);
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is GridLength { IsAbsolute: true } g ? g.Value : BindingOperations.DoNothing;
}