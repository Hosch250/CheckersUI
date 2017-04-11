using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using CheckersUI.Facade;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class BoardEditor : IResizable
    {
        private Image _draggedImage;
        private Piece _piece;
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        private BoardEditorViewModel ViewModel => (BoardEditorViewModel)DataContext;

        public BoardEditor()
        {
            InitializeComponent();

            _currentTheme = (string)_roamingSettings.Values["Theme"];
            ApplicationData.Current.DataChanged += Current_DataChanged;
            LoadImages();
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
                LoadImages();
            });
        }

        private void LoadImages()
        {
            var whiteCheckerBitmapImage = new BitmapImage(GetPieceUri(Piece.WhiteChecker));
            WhiteChecker.Source = whiteCheckerBitmapImage;

            var whiteKingBitmapImage = new BitmapImage(GetPieceUri(Piece.WhiteKing));
            WhiteKing.Source = whiteKingBitmapImage;

            var blackCheckerBitmapImage = new BitmapImage(GetPieceUri(Piece.BlackChecker));
            BlackChecker.Source = blackCheckerBitmapImage;

            var blackKingBitmapImage = new BitmapImage(GetPieceUri(Piece.BlackKing));
            BlackKing.Source = blackKingBitmapImage;
        }

        private Uri GetPieceUri(Piece piece)
        {
            if (piece == null) { return null; }

            if (piece.Equals(Piece.WhiteChecker))
            {
                return new Uri($"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/WhiteChecker.png", UriKind.Absolute);
            }

            if (piece.Equals(Piece.WhiteKing))
            {
                return new Uri($"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/WhiteKing.png", UriKind.Absolute);
            }

            if (piece.Equals(Piece.BlackChecker))
            {
                return new Uri($"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/BlackChecker.png", UriKind.Absolute);
            }

            if (piece.Equals(Piece.BlackKing))
            {
                return new Uri($"ms-appx:///Assets/{_roamingSettings.Values["Theme"]}Theme/BlackKing.png", UriKind.Absolute);
            }

            throw new MissingMemberException("Piece not found");
        }

        private Piece GetPiece(Image image)
        {
            if (image == WhiteChecker)
            {
                return Piece.WhiteChecker;
            }
            else if (image == WhiteKing)
            {
                return Piece.WhiteKing;
            }
            else if (image == BlackChecker)
            {
                return Piece.BlackChecker;
            }
            else
            {
                return Piece.BlackKing;
            }
        }

        private void PlacePiece(Point point)
        {
            var row = (int)Math.Floor(point.Y / (BoardGrid.ActualHeight / 8));
            var column = (int)Math.Floor(point.X / (BoardGrid.ActualWidth / 8));

            ViewModel.AddPiece(_piece, row, column);
        }

        private void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var source = ((Image) sender).Source;
            _draggedImage = new Image {Source = source};
            _piece = GetPiece((Image) sender);

            Canvas.Children.Add(_draggedImage);
            SetPosition(e.GetCurrentPoint(Canvas).Position);
            Canvas.CapturePointer(e.Pointer);
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            PlacePiece(e.GetCurrentPoint(BoardGrid).Position);
            Canvas.Children.Remove(_draggedImage);
            Canvas.ReleasePointerCapture(e.Pointer);

            _draggedImage = null;
            _piece = null;

            ViewModel.UpdateFen();
        }

        private void SetPosition(Point point)
        {
            Canvas.SetLeft(_draggedImage, point.X - (_draggedImage.ActualWidth / 2));
            Canvas.SetTop(_draggedImage, point.Y - (_draggedImage.ActualHeight / 2));
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_draggedImage != null)
            {
                SetPosition(e.GetCurrentPoint(Canvas).Position);
            }
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(BoardGrid).Position;
            var row = (int)Math.Floor(point.Y / (BoardGrid.ActualHeight / 8));
            var column = (int)Math.Floor(point.X / (BoardGrid.ActualWidth / 8));
            if (!Board.IsValidSquare(ViewModel.Variant, row, column)) { return; }

            var piece = ViewModel.Board[row, column];
            if (piece == null) { return; }

            Canvas.CapturePointer(e.Pointer);
            var bitmapImage = new BitmapImage(GetPieceUri(piece));
            var image = new Image {Source = bitmapImage};
            _draggedImage = image;
            _piece = piece;

            ViewModel.RemovePiece(row, column);
            Canvas.Children.Add(_draggedImage);
            SetPosition(point);
        }

        private void CommandBar_Closed(object sender, object e) =>
            ViewModel.DisplayAppBarCommand.Execute(null);

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
            ((ComboBox)sender).SelectedIndex = 1;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != 0 && _currentLayout == PageLayout.Small)
            {
                SetPieceWidth();
            }
        }

        private PageLayout _currentLayout;
        public void LoadLayout(PageLayout layout)
        {
            _currentLayout = layout;

            switch (layout)
            {
                case PageLayout.Default:
                    LoadDefaultLayout();
                    break;
                case PageLayout.Small:
                    LoadSmallLayout();
                    break;
                default:
                    throw new ArgumentException(nameof(layout));
            }
        }

        private void SetPieceWidth()
        {
            // adjust for size 10 margins
            var pieceWidth = (ActualWidth - 20) / 8;

            WhiteChecker.Width = pieceWidth;
            WhiteKing.Width = pieceWidth;
            BlackChecker.Width = pieceWidth;
            BlackKing.Width = pieceWidth;
        }

        private void LoadSmallLayout()
        {
            foreach (var row in MasterGrid.RowDefinitions)
            {
                row.Height = GridLength.Auto;
            }

            MasterGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);

            MasterGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MasterGrid.ColumnDefinitions[1].Width = new GridLength(0);
            MasterGrid.ColumnDefinitions[2].Width = new GridLength(0);

            MasterGrid.Margin = new Thickness(10, 0, 10, 0);

            foreach (var col in PieceContainer.ColumnDefinitions)
            {
                col.Width = new GridLength(1, GridUnitType.Star);
            }

            PieceContainer.RowDefinitions[1].Height = new GridLength(0);
            PieceContainer.RowDefinitions[2].Height = new GridLength(0);
            PieceContainer.RowDefinitions[3].Height = new GridLength(0);
            PieceContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
            PieceContainer.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetRow(BlackKingGroup, 0);
            Grid.SetColumn(BlackKingGroup, 1);
            Grid.SetRow(WhiteCheckerGroup, 0);
            Grid.SetColumn(WhiteCheckerGroup, 2);
            Grid.SetRow(WhiteKingGroup, 0);
            Grid.SetColumn(WhiteKingGroup, 3);

            BlackCheckerLabel.Text = "B. Checker";
            BlackKingLabel.Text = "B. King";
            WhiteCheckerLabel.Text = "W. Checker";
            WhiteKingLabel.Text = "W. King";

            Grid.SetRow(BoardGrid, 1);
            Grid.SetColumn(BoardGrid, 0);

            Grid.SetRow(FenContainer, 2);
            Grid.SetColumn(FenContainer, 0);

            OptionsPane.Visibility = Visibility.Collapsed;

            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
        }

        private void LoadDefaultLayout()
        {
            PieceContainer.RowDefinitions[0].Height = GridLength.Auto;
            PieceContainer.RowDefinitions[1].Height = new GridLength(640);
            PieceContainer.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);

            MasterGrid.ColumnDefinitions[0].Width = new GridLength(190);
            MasterGrid.ColumnDefinitions[1].Width = new GridLength(640);
            MasterGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);

            MasterGrid.Margin = new Thickness(0);

            foreach (var row in PieceContainer.RowDefinitions)
            {
                row.Height = new GridLength(1, GridUnitType.Star);
            }

            PieceContainer.ColumnDefinitions[1].Width = new GridLength(0);
            PieceContainer.ColumnDefinitions[2].Width = new GridLength(0);
            PieceContainer.ColumnDefinitions[3].Width = new GridLength(0);
            PieceContainer.HorizontalAlignment = HorizontalAlignment.Right;

            Grid.SetRow(BlackKingGroup, 1);
            Grid.SetColumn(BlackKingGroup, 0);
            Grid.SetRow(WhiteCheckerGroup, 2);
            Grid.SetColumn(WhiteCheckerGroup, 0);
            Grid.SetRow(WhiteKingGroup, 3);
            Grid.SetColumn(WhiteKingGroup, 0);

            BlackCheckerLabel.Text = "Black Checker";
            BlackKingLabel.Text = "Black King";
            WhiteCheckerLabel.Text = "White Checker";
            WhiteKingLabel.Text = "White King";

            Grid.SetRow(BoardGrid, 0);
            Grid.SetColumn(BoardGrid, 1);
            BoardGrid.MaxHeight = 640;

            Grid.SetRow(FenContainer, 1);
            Grid.SetColumn(FenContainer, 1);
            FenContainer.MaxWidth = 640;

            OptionsPane.Visibility = Visibility.Visible;

            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
        }
    }
}
