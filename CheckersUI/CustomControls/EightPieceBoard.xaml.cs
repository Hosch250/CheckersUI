using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using CheckersUI.Facade;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace CheckersUI.CustomControls
{
    public sealed partial class EightPieceBoard
    {
        private ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(nameof(Board), typeof(Board), typeof(EightPieceBoard), new PropertyMetadata(null, (sender, e) => ((EightPieceBoard)sender).LoadPieces(e.OldValue as Board, e.NewValue as Board)));

        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register(nameof(Selection), typeof(Coord), typeof(EightPieceBoard), null);

        public EightPieceBoard()
        {
            InitializeComponent();
            LoadBoard();

            _currentTheme = (string)_roamingSettings.Values["Theme"];
            ApplicationData.Current.DataChanged += Current_DataChanged;
        }

        private Uri GetPieceUri(Piece piece)
        {
            if (piece.Equals(Piece.WhiteChecker))
            {
                return new Uri($"ms-appx:///../Assets/{_roamingSettings.Values["Theme"]}Theme/WhiteChecker.png", UriKind.Absolute);
            }

            if (piece.Equals(Piece.WhiteKing))
            {
                return new Uri($"ms-appx:///../Assets/{_roamingSettings.Values["Theme"]}Theme/WhiteKing.png", UriKind.Absolute);
            }

            if (piece.Equals(Piece.BlackChecker))
            {
                return new Uri($"ms-appx:///../Assets/{_roamingSettings.Values["Theme"]}Theme/BlackChecker.png", UriKind.Absolute);
            }

            if (piece.Equals(Piece.BlackKing))
            {
                return new Uri($"ms-appx:///../Assets/{_roamingSettings.Values["Theme"]}Theme/BlackKing.png", UriKind.Absolute);
            }

            throw new MissingMemberException("Piece not found");
        }

        private void DeleteBoard()
        {
            foreach (var item in BoardGrid.Children.ToList())
            {
                if (((FrameworkElement)item).Name == "BoardImage")
                {
                    BoardGrid.Children.Remove(item);
                    return;
                }
            }
        }

        private void LoadBoard()
        {
            var uri = new Uri($"ms-appx:///../Assets/{_roamingSettings.Values["Theme"]}Theme/Checkerboard.png", UriKind.Absolute);
            var bitmapImage = new BitmapImage { UriSource = uri };

            var image = new Image { Source = bitmapImage };
            image.Name = "BoardImage";
            Grid.SetRowSpan(image, 8);
            Grid.SetColumnSpan(image, 8);

            DeleteBoard();
            BoardGrid.Children.Add(image);
        }

        private string _currentTheme;
        private void Current_DataChanged(ApplicationData sender, object args)
        {
            if ((string)_roamingSettings.Values["Theme"] == _currentTheme)
            {
                return;
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _currentTheme = (string)_roamingSettings.Values["Theme"];
                LoadBoard();
                LoadPieces(null, Board);
            });
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

            var row = (int)Math.Floor(point.Y / (BoardGrid.ActualHeight / 8));
            var column = (int)Math.Floor(point.X / (BoardGrid.ActualWidth / 8));

            Selection = new Coord(row, column);
        }

        private void ClearPieces(Board board)
        {
            foreach (Image element in BoardGrid.Children.ToList())
            {
                if (element.Name == "BoardImage") { continue; }

                var row = Grid.GetRow(element);
                var column = Grid.GetColumn(element);
                var uri = ((BitmapImage)element.Source).UriSource.AbsolutePath;

                if (Board.GameBoard[row][column] == null || GetPieceUri(Board.GameBoard[row][column]).AbsolutePath != uri)
                {
                    BoardGrid.Children.Remove(element);
                }
            }
        }

        private void PlaceChecker(Piece piece, int row, int column)
        {
            var bitmapImage = new BitmapImage {UriSource = GetPieceUri(piece)};

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
