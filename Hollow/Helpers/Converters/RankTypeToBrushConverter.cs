using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Hollow.Helpers.Converters;

public class RankTypeToBrushConverter : IValueConverter
{
    public static readonly RankTypeToBrushConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, 
        CultureInfo culture)
    {
        if (value is string rankType && targetType.IsAssignableTo(typeof(IBrush)))
        {
            return rankType switch
            {
                "3" => Brushes.MediumPurple,
                "4" => Brushes.Gold,
                _ => Brushes.White
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