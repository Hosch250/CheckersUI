﻿using Checkers;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace CheckersUI
{
    public sealed partial class MainPage
    {
        private static Dictionary<Piece.Piece, Uri> PieceToUriMap = new Dictionary<Piece.Piece, Uri>
            {
                {Piece.whiteChecker.Value, new Uri("ms-appx:///Assets/WhiteChecker.png", UriKind.Absolute)},
                {Piece.whiteKing.Value, new Uri("ms-appx:///Assets/WhiteKing.png", UriKind.Absolute)},
                {Piece.blackChecker.Value, new Uri("ms-appx:///Assets/BlackChecker.png", UriKind.Absolute)},
                {Piece.blackKing.Value, new Uri("ms-appx:///Assets/BlackKing.png", UriKind.Absolute)}
            };

        public MainPage()
        {
            InitializeComponent();
        }

        private void ClearPieces(FSharpList<FSharpList<FSharpOption<Piece.Piece>>> board)
        {
            foreach (Image element in Board.Children.ToList())
            {
                if (element != BoardImage)
                {
                    var row = Grid.GetRow(element);
                    var column = Grid.GetColumn(element);
                    var uri = ((BitmapImage)element.Source).UriSource.AbsolutePath;

                    if (board[row][column] == FSharpOption<Piece.Piece>.None || PieceToUriMap[board[row][column].Value].AbsolutePath != uri)
                    {
                        Board.Children.Remove(element);
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
            Board.Children.Add(image);
        }

        public void UpdateBoard(GameController.GameController gameController)
        {
            var board = gameController.Board;
            ClearPieces(board);

            for (var rowIndex = 0; rowIndex < board.Length; rowIndex++)
            {
                for (var colIndex = 0; colIndex < board[rowIndex].Length; colIndex++)
                {
                    var piece = board[rowIndex][colIndex];
                    if (piece == FSharpOption<Piece.Piece>.None)
                    {
                        continue;
                    }

                    PlaceChecker(piece.Value, rowIndex, colIndex);
                }
            }
        }

        private void Board_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((Grid)sender).Position;

            var row = (int)Math.Floor(point.Y / 80);
            var column = (int)Math.Floor(point.X / 80);

            ((MainPageViewModel)DataContext).Selection = new Types.Coord(row, column);
        }
    }
}
