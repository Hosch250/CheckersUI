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
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace CheckersUI.CustomControls
{
    public sealed partial class EightPieceBoard
    {
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        public static readonly DependencyProperty BoardProperty =
            DependencyProperty.Register(nameof(Board), typeof(Board), typeof(EightPieceBoard),
                new PropertyMetadata(null, (sender, e) => ((EightPieceBoard)sender).LoadPieces(e.OldValue as Board, e.NewValue as Board)));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Player), typeof(EightPieceBoard),
                new PropertyMetadata(null, (sender, e) => ((EightPieceBoard)sender).LoadPieces(null, ((EightPieceBoard)sender).Board)));

        public static readonly DependencyProperty MoveCommand =
            DependencyProperty.Register(nameof(Move), typeof(ICommand), typeof(EightPieceBoard), null);

        public EightPieceBoard()
        {
            InitializeComponent();
            Orientation = Player.Black;

            _currentTheme = (string)_roamingSettings.Values["Theme"];
            ApplicationData.Current.DataChanged += DataChanged;

            LoadBoard();
        }

        public Board Board
        {
            get { return (Board)GetValue(BoardProperty); }
            set { SetValue(BoardProperty, value); }
        }

        private Coord _selection;
        public Coord Selection
        {
            get { return _selection; }
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnSelectionChanged(value);
                }
            }
        }

        public Player Orientation
        {
            get { return (Player)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public ICommand Move
        {
            get { return (ICommand)GetValue(MoveCommand); }
            set
            {
                SetValue(MoveCommand, value);
            }
        }

        private SolidColorBrush GetBorderBrush()
        {
            var theme = (Theme)Enum.Parse(typeof(Theme), _currentTheme);
            switch (theme)
            {
                case Theme.Wood:
                    return new SolidColorBrush(Colors.OrangeRed);
                case Theme.Steel:
                    return new SolidColorBrush(Colors.Blue);
                case Theme.Plastic:
                    return new SolidColorBrush(Colors.LightGreen);
                default:
                    throw new ArgumentException(nameof(theme));
            }
        }

        public void SetBorder(Coord value)
        {
            var pieceBorder = new Border
            {
                BorderBrush = GetBorderBrush(),
                BorderThickness = new Thickness(2)
            };
            Grid.SetRow(pieceBorder, AdjustedIndex(value.Row));
            Grid.SetColumn(pieceBorder, AdjustedIndex(value.Column));
            Canvas.SetZIndex(pieceBorder, 1);

            BoardGrid.Children.Add(pieceBorder);
        }

        public void ClearBorders()
        {
            foreach (var border in BoardGrid.Children.OfType<Border>().ToList())
            {
                BoardGrid.Children.Remove(border);
            }
        }

        private double GetPromptSize() => (ActualHeight / 8) * .25;

        public void SetPrompt(Coord value)
        {
            var prompt = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Gray),
                Opacity = 80,
                Height = GetPromptSize(),
                Width = GetPromptSize()
            };
            Grid.SetRow(prompt, AdjustedIndex(value.Row));
            Grid.SetColumn(prompt, AdjustedIndex(value.Column));
            Canvas.SetZIndex(prompt, 1);

            BoardGrid.Children.Add(prompt);
        }

        public void ClearPrompts()
        {
            foreach (var prompt in BoardGrid.Children.OfType<Ellipse>().ToList())
            {
                BoardGrid.Children.Remove(prompt);
            }
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

        private void DataChanged(ApplicationData sender, object args)
        {
            UpdateTheme();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if ((string)_roamingSettings.Values["EnableMoveHints"] == bool.FalseString)
                {
                    ClearBorders();
                }
            });
        }

        private string _currentTheme;
        private void UpdateTheme()
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
                RecolorBorders();
            });
        }

        private void RecolorBorders()
        {
            foreach (var border in BoardGrid.Children.OfType<Border>().ToList())
            {
                border.BorderBrush = GetBorderBrush();
            }
        }

        private void BoardGridPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((Grid)sender).Position;

            var row = (int)Math.Floor(point.Y / (BoardGrid.ActualHeight / 8));
            var column = (int)Math.Floor(point.X / (BoardGrid.ActualWidth / 8));

            var fromCoord = Selection;
            var toCoord = new Coord(AdjustedIndex(row), AdjustedIndex(column));

            if (fromCoord != null && Move != null && Move.CanExecute(new { fromCoord, toCoord }))
            {
                Move.Execute(new {fromCoord, toCoord});
                Selection = null;
            }
            else
            {
                Selection = toCoord;
            }
        }

        private void ClearPieces()
        {
            foreach (var element in BoardGrid.Children.OfType<Image>().ToList())
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
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_adjustSize) { return; }
            if (Math.Abs(BoardGrid.ActualWidth - DesiredSize.Width) < 1)
            {
                return;
            }
            
            _adjustSize = false;
            BoardGrid.Height = DesiredSize.Width - (Margin.Left + Margin.Right);
            BoardGrid.Width = DesiredSize.Height - (Margin.Top + Margin.Bottom);
            _adjustSize = true;

            foreach (var prompt in BoardGrid.Children.OfType<Ellipse>().ToList())
            {
                prompt.Height = GetPromptSize();
                prompt.Width = GetPromptSize();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var minAvailableSize = Math.Min(availableSize.Width, availableSize.Height);
            return new Size(minAvailableSize, minAvailableSize);
        }

        public event EventHandler<Coord> SelectionChanged;
        private void OnSelectionChanged(Coord arg) => SelectionChanged?.Invoke(this, arg);
    }
}
