using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CheckersUI.Facade;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class BoardEditor
    {
        private Image _draggedImage;
        private Piece _piece;

        public BoardEditor()
        {
            InitializeComponent();
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

            if (row < 0 || column < 0 || row > 7 || column > 7)
            {
                return;
            }

            var dataContext = (BoardEditorViewModel) DataContext;
            dataContext.AddPiece(_piece, row, column);
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
    }
}
