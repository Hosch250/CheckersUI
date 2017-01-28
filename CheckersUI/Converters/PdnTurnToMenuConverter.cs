using System;
using Windows.UI.Xaml.Data;
using CheckersUI.Facade;

namespace CheckersUI.Converters
{
    public class PdnTurnToMenuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var turn = (PdnTurn)value;
            var move = turn.WhiteMove ?? turn.BlackMove;

            var converter = new PdnMoveToDisplayStringConverter();
            return "☰ " + converter.Convert(move, null, null, language);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}