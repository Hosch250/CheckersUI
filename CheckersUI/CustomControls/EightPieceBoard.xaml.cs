using Checkers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using static Checkers.Types;

namespace CheckersUI.CustomControls
{
    public sealed partial class EightPieceBoard : INotifyPropertyChanged
    {
        private static Dictionary<Piece.Piece, Uri> PieceToUriMap = new Dictionary<Piece.Piece, Uri>
            {
                {Piece.whiteChecker.Value, new Uri("ms-appx:///../Assets/WhiteChecker.png", UriKind.Absolute)},
                {Piece.whiteKing.Value, new Uri("ms-appx:///../Assets/WhiteKing.png", UriKind.Absolute)},
                {Piece.blackChecker.Value, new Uri("ms-appx:///../Assets/BlackChecker.png", UriKind.Absolute)},
                {Piece.blackKing.Value, new Uri("ms-appx:///../Assets/BlackKing.png", UriKind.Absolute)}
            };

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(nameof(Board),
                typeof(IEnumerable<IEnumerable<FSharpOption<Piece.Piece>>>),
                typeof(EightPieceBoard),
                new PropertyMetadata(null, new PropertyChangedCallback((sender, e) => ((EightPieceBoard)sender).LoadPieces())));

        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register(nameof(Selection), typeof(Coord), typeof(EightPieceBoard), null);

        public EightPieceBoard()
        {
            InitializeComponent();
        }

        public IEnumerable<IEnumerable<FSharpOption<Piece.Piece>>> Board
        {
            get { return (IEnumerable<IEnumerable<FSharpOption<Piece.Piece>>>)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        public Coord Selection
        {
            get { return (Coord)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }

        private void Board_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((Grid)sender).Position;

            var row = (int)Math.Floor(point.Y / 80);
            var column = (int)Math.Floor(point.X / 80);

            Selection = new Coord(row, column);
        }

        private void ClearPieces(IEnumerable<IEnumerable<FSharpOption<Piece.Piece>>> board)
        {
            foreach (Image element in BoardGrid.Children.ToList())
            {
                if (element != BoardImage)
                {
                    var row = Grid.GetRow(element);
                    var column = Grid.GetColumn(element);
                    var uri = ((BitmapImage)element.Source).UriSource.AbsolutePath;

                    if (Board.ElementAt(row).ElementAt(column) == FSharpOption<Piece.Piece>.None || PieceToUriMap[Board.ElementAt(row).ElementAt(column).Value].AbsolutePath != uri)
                    {
                        BoardGrid.Children.Remove(element);
                    }
                }
            }
        }

        private void PlaceChecker(Piece.Piece piece, int row, int column)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.UriSource = PieceToUriMap[piece];

            var image = new Image();
            image.Source = bitmapImage;
            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);
            BoardGrid.Children.Add(image);
        }

        public void LoadPieces()
        {
            ClearPieces(Board);

            for (var rowIndex = 0; rowIndex < Board.Count(); rowIndex++)
            {
                for (var colIndex = 0; colIndex < Board.ElementAt(rowIndex).Count(); colIndex++)
                {
                    var piece = Board.ElementAt(rowIndex).ElementAt(colIndex);
                    if (piece == FSharpOption<Piece.Piece>.None)
                    {
                        continue;
                    }

                    PlaceChecker(piece.Value, rowIndex, colIndex);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
