using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Hollow.Helpers.Converters;

public class RankTypeToFormattedConverter: IValueConverter
{
    public static readonly RankTypeToFormattedConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, 
        CultureInfo culture)
    {
        if (value is string rankType && targetType.IsAssignableTo(typeof(string)))
        {
            return rankType switch
            {
                "3" => "A",
                "4" => "S",
                _ => "B"
            };
        }
        return new BindingNotification(new InvalidCastException(), 
            BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, 
        object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}