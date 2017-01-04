using System;
using Windows.UI.Xaml.Data;

namespace CheckersUI.Converters
{
    public class IntegerToListItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (int)value + ".";

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            value;
    }
}