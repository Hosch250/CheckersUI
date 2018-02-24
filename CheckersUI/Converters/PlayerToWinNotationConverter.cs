using System;
using Windows.UI.Xaml.Data;
using CheckersUI.Enums;

namespace CheckersUI.Converters
{
    public class PlayerToWinNotationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (Status)value;
            switch (status)
            {
                case Status.WhiteWin:
                    return "1 - 0";
                case Status.BlackWin:
                    return "0 - 1";
                case Status.Drawn:
                    return "½ - ½";
                case Status.InProgress:
                    return "*";
                default:
                    throw new ArgumentException(nameof(status));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}