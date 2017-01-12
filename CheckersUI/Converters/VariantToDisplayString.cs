using System;
using Windows.UI.Xaml.Data;
using CheckersUI.Facade;

namespace CheckersUI.Converters
{
    public class VariantToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var variant = (Variant)value;
            switch (variant)
            {
                case Variant.AmericanCheckers:
                    return "American Checkers";
                case Variant.PoolCheckers:
                    return "Pool Checkers";
                default:
                    throw new ArgumentException(nameof(variant));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;
            switch (str)
            {
                case "American Checkers":
                    return Variant.AmericanCheckers;
                case "Pool Checkers":
                    return Variant.PoolCheckers;
                default:
                    throw new ArgumentException(nameof(str));
            }
        }
    }
}