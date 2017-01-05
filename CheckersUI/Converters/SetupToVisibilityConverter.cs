using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CheckersUI.Converters
{
    public class SetupToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (Setup) value == Setup.Default ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (Visibility)value == Visibility.Collapsed ? Setup.Default : Setup.FromPosition;
    }
}