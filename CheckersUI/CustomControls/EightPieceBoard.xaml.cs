using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using CheckersUI.Facade;
using Windows.Storage;
using Windows.UI.Core;

namespace CheckersUI.CustomControls
{
    public sealed partial class EightPieceBoard
    {
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(nameof(Board), typeof(Board), typeof(EightPieceBoard),
                new PropertyMetadata(null, (sender, e) => ((EightPieceBoard)sender).LoadPieces(e.OldValue as Board, e.NewValue as Board)));

        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.Register(nameof(Selection), typeof(Coord), typeof(EightPieceBoard), null);

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Player), typeof(EightPieceBoard),
                new PropertyMetadata(null, (sender, e) => ((EightPieceBoard)sender).LoadPieces(null, ((EightPieceBoard)sender).Board)));

        public EightPieceBoard()
        {
            InitializeComponent();

            _currentTheme = (string)_roamingSettings.Values["Theme"];
            ApplicationData.Current.DataChanged += Current_DataChanged;

            LoadBoard();
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

        public Player Orientation
        {
            get { return (Player)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        private string GetPieceUriPath(Piece piece)
        {
            if (piece == null) { return null; }

            if (piece.Equals(Piece.WhiteChecker))
            {
                return $"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/WhiteChecker.png";
            }

            if (piece.Equals(Piece.WhiteKing))
            {
                return $"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/WhiteKing.png";
            }

            if (piece.Equals(Piece.BlackChecker))
            {
                return $"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/BlackChecker.png";
            }

            if (piece.Equals(Piece.BlackKing))
            {
                return $"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/BlackKing.png";
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
            var uri = new Uri($"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/Checkerboard.png", UriKind.Absolute);
            var bitmapImage = new BitmapImage { UriSource = uri };

            var image = new Image
            {
                Source = bitmapImage,
                Name = "BoardImage"
            };

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

        private void BoardGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((Grid)sender).Position;

            var row = (int)Math.Floor(point.Y / (BoardGrid.ActualHeight / 8));
            var column = (int)Math.Floor(point.X / (BoardGrid.ActualWidth / 8));

            Selection = new Coord(AdjustedIndex(row), AdjustedIndex(column));
        }

        private void ClearPieces()
        {
            foreach (Image element in BoardGrid.Children.ToList())
            {
                if (element.Name == "BoardImage") { continue; }

                var row = AdjustedIndex(Grid.GetRow(element));
                var column = AdjustedIndex(Grid.GetColumn(element));
                var uri = ((BitmapImage)element.Source).UriSource.AbsoluteUri;

                if (GetPieceUriPath(Board.GameBoard[row, column]) != uri)
                {
                    BoardGrid.Children.Remove(element);
                }
            }
        }

        private void PlaceChecker(Piece piece, int row, int column)
        {
            var bitmapImage = new BitmapImage {UriSource = new Uri(GetPieceUriPath(piece), UriKind.Absolute)};

            var image = new Image {Source = bitmapImage};
            Grid.SetRow(image, row);
            Grid.SetColumn(image, column);
            BoardGrid.Children.Add(image);
        }

        public void LoadPieces(Board oldValue, Board newValue)
        {
            if (newValue == null) { return; }
            
            ClearPieces();

            for (var rowIndex = 0; rowIndex < 8; rowIndex++)
            {
                for (var colIndex = 0; colIndex < 8; colIndex++)
                {
                    var piece = newValue.GameBoard[rowIndex, colIndex];
                    if (piece == null || piece.Equals(oldValue?.GameBoard[rowIndex, colIndex]))
                    {
                        continue;
                    }

                    PlaceChecker(piece, AdjustedIndex(rowIndex), AdjustedIndex(colIndex));
                }
            }
        }

        private int AdjustedIndex(int index) =>
            Orientation == Player.White ? index : 7 - index;

        private bool _adjustSize = true;
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_adjustSize) { return; }
            if (Math.Abs(ActualWidth - ActualHeight) < 1)
            {
                return;
            }
            
            _adjustSize = false;
            BoardGrid.MaxHeight = DesiredSize.Width;
            BoardGrid.MaxWidth = DesiredSize.Height;
            _adjustSize = true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var minAvailableSize = Math.Min(availableSize.Width, availableSize.Height);
            return new Size(minAvailableSize, minAvailableSize);
        }
    }
}
