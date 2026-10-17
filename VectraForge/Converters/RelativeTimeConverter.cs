using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VectraForge.Converters;

public class RelativeTimeConverter : IValueConverter
{
    private readonly TimeProvider _time;

    public RelativeTimeConverter() : this(TimeProvider.System)
    {
    }
    
    public RelativeTimeConverter(TimeProvider time) => _time = time;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTimeOffset dto) return value;
        var delta = _time.GetUtcNow() - dto;

        return delta switch
        {
            { TotalMinutes: < 1 } => "Just now",
            { TotalMinutes: < 60 } => $"{(int)delta.TotalMinutes} min ago",
            { TotalHours: < 24 } => $"{(int)delta.TotalHours} hr ago",
            { TotalDays: < 7 } => $"{(int)delta.TotalDays} day{((int)delta.TotalDays == 1 ? "" : "s")} ago",
            _ => dto.LocalDateTime.ToString("MMM d, yyyy", culture)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}