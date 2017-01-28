using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;
using CheckersUI.Facade;

namespace CheckersUI.Converters
{
    public class PlayerToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((Player) value)
            {
                case Player.White:
                    return (string) parameter == nameof(Player.White) ? FontWeights.Bold : FontWeights.Normal;
                case Player.Black:
                    return (string)parameter == nameof(Player.Black) ? FontWeights.Bold : FontWeights.Normal;
                default:
                    return FontWeights.Bold;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}