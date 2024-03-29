﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dicidea.Core.Converters
{
    /// <summary>
    ///     Wandelt einen bool in eine Sichtbarkeit um.
    /// </summary>
    public class ConverterVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Collapsed;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return visibility == Visibility.Visible;
            return value;
        }
    }
}
