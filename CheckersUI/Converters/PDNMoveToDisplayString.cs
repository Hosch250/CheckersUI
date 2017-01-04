using System;
using Windows.UI.Xaml.Data;
using CheckersUI.Facade;

namespace CheckersUI.Converters
{
    public class PDNMoveToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var move = (PDNMove)value;

            if (move == null) { return string.Empty; }
            if (move.Move.Count <= 3) { return move.DisplayString; }
            return move.Move[0] + "…" + move.Move[move.Move.Count - 1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            value;
    }
}