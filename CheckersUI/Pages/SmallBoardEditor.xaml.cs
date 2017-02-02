using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using CheckersUI.Facade;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class SmallBoardEditor
    {
        private Image _draggedImage;
        private Piece _piece;
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        private SmallBoardEditorViewModel ViewModel => (SmallBoardEditorViewModel)DataContext;

        public SmallBoardEditor()
        {
            InitializeComponent();

            _currentTheme = (string)_roamingSettings.Values["Theme"];
            ApplicationData.Current.DataChanged += Current_DataChanged;
            LoadImages();

            SizeChanged += SmallBoardEditor_SizeChanged;
        }

        private void SmallBoardEditor_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            // adjust for size 10 margins
            var pieceWidth = (ActualWidth - 20) / 8;

            WhiteChecker.Width = pieceWidth;
            WhiteKing.Width = pieceWidth;
            BlackChecker.Width = pieceWidth;
            BlackKing.Width = pieceWidth;
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

            // todo let user set variant they are working with
            if (!Board.IsValidSquare(ViewModel.Variant, row, column)) { return; }

            ViewModel.AddPiece(_piece, row, column);
        }

        private void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var source = ((Image)sender).Source;
            _draggedImage = new Image
            {
                Source = source,
                // adjust for size 10 margins
                Width = (ActualWidth - 20) / 8
            };

            _piece = GetPiece((Image)sender);

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
            var image = new Image
            {
                Source = bitmapImage,
                // adjust for size 10 margins
                Width = (ActualWidth - 20) / 8
            };

            _draggedImage = image;

            _piece = piece;

            ViewModel.RemovePiece(row, column);
            Canvas.Children.Add(_draggedImage);

            SetPosition(point);
        }

        private void CommandBar_Closed(object sender, object e)
        {
            ViewModel.DisplayAppBarCommand.Execute(null);
        }
    }
}
