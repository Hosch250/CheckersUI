﻿using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using CheckersUI.Facade;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class BoardEditor
    {
        private Image _draggedImage;
        private Piece _piece;
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        private BoardEditorViewModel ViewModel => (BoardEditorViewModel)DataContext;

        public BoardEditor()
        {
            InitializeComponent();
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

            if (!Board.IsValidSquare(row, column)) { return; }

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
            if (!Board.IsValidSquare(row, column)) { return; }

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
    }
}
