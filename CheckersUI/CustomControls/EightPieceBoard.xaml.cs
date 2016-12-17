using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using CheckersUI.Facade;

namespace CheckersUI.CustomControls
{
    public sealed partial class EightPieceBoard
    {
        private static Dictionary<Piece, Uri> PieceToUriMap = new Dictionary<Piece, Uri>
            {
                {Piece.WhiteChecker, new Uri("ms-appx:///../Assets/WhiteChecker.png", UriKind.Absolute)},
                {Piece.WhiteKing, new Uri("ms-appx:///../Assets/WhiteKing.png", UriKind.Absolute)},
                {Piece.BlackChecker, new Uri("ms-appx:///../Assets/BlackChecker.png", UriKind.Absolute)},
                {Piece.BlackKing, new Uri("ms-appx:///../Assets/BlackKing.png", UriKind.Absolute)}
            };

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(nameof(Board),
                typeof(Board),
                typeof(EightPieceBoard),
                new PropertyMetadata(null, (sender, e) => ((EightPieceBoard)sender).LoadPieces(e.OldValue as Board, e.NewValue as Board)));

        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register(nameof(Selection), typeof(Coord), typeof(EightPieceBoard), null);

        public EightPieceBoard()
        {
            InitializeComponent();
        }

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        public Coord Selection
        {
            get { return (Coord)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }

        private void BoardGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((Grid)sender).Position;

            var row = (int)Math.Floor(point.Y / 80);
            var column = (int)Math.Floor(point.X / 80);

            Selection = new Coord(row, column);
        }

        private void ClearPieces(Board board)
        {
            foreach (Image element in BoardGrid.Children.ToList())
            {
                if (element == BoardImage) { continue; }

                var row = Grid.GetRow(element);
                var column = Grid.GetColumn(element);
                var uri = ((BitmapImage)element.Source).UriSource.AbsolutePath;

                if (Board.GameBoard[row][column] == null || PieceToUriMap[Board.GameBoard[row][column]].AbsolutePath != uri)
                {
                    BoardGrid.Children.Remove(element);
                }
            }
        }

        private void PlaceChecker(Piece piece, int row, int column)
        {
            var bitmapImage = new BitmapImage {UriSource = PieceToUriMap[piece]};

            var image = new Image {Source = bitmapImage};
            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);
            BoardGrid.Children.Add(image);
        }

        public void LoadPieces(Board oldValue, Board newValue)
        {
            ClearPieces(Board);

            for (var rowIndex = 0; rowIndex < Board.GameBoard.Count; rowIndex++)
            {
                for (var colIndex = 0; colIndex < Board.GameBoard[rowIndex].Count; colIndex++)
                {
                    var piece = Board.GameBoard[rowIndex][colIndex];
                    if (piece == null || newValue.GameBoard[rowIndex][colIndex].Equals(oldValue?.GameBoard[rowIndex][colIndex]))
                    {
                        continue;
                    }

                    PlaceChecker(piece, rowIndex, colIndex);
                }
            }
        }
    }
}
