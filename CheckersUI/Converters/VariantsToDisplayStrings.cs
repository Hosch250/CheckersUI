using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using CheckersUI.Facade;

namespace CheckersUI.Converters
{
    public class VariantsToDisplayStringsConverter : IValueConverter
    {
        private string VariantToString(Variant variant)
        {
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

        private Variant StringToVariant(string str)
        {
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

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var variants = (IEnumerable<Variant>)value;
            return variants.Select(VariantToString);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var strings = (IEnumerable<string>)value;
            return strings.Select(StringToVariant);
        }
    }
}