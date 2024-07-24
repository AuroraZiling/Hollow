using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace Hollow.Helpers.Converters;

public class BitmapValueConverter : IValueConverter
{
    public static BitmapValueConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName && targetType == typeof(Bitmap))
        {
            return new Bitmap(fileName);
        }

        throw new NotSupportedException();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}