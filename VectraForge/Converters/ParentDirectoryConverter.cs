using System;
using System.Globalization;
using System.IO;
using Avalonia.Data.Converters;

namespace VectraForge.Converters;

public class ParentDirectoryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string path ? Path.GetDirectoryName(path) : value;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}