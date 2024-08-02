﻿using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Hollow.Views.Controls.ProgressRing
{
    public class FitSquarelyWithinAspectRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Rect bounds = (Rect)value;
            return Math.Min(bounds.Width, bounds.Height);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
