using System;
using Windows.UI.Xaml.Data;
using CheckersUI.Facade;

namespace CheckersUI.Converters
{
    public class PlayerToWinNotationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var player = (Player?)value;

            if (!player.HasValue) { return "*"; }

            return player.Value == Player.Black ? "0 - 1" : "1 - 0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}