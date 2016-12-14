using Checkers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CheckersUI
{
    public sealed partial class MainPage
    {
        private static Dictionary<Tuple<Player, PieceType>, Uri> pieceToUriMap = new Dictionary<Tuple<Player, PieceType>, Uri>
            {
                {Tuple.Create(Player.White, PieceType.Checker), new Uri("ms-appx:///Assets/WhiteChecker.png", UriKind.Absolute)},
                {Tuple.Create(Player.White, PieceType.King), new Uri("ms-appx:///Assets/WhiteKing.png", UriKind.Absolute)},
                {Tuple.Create(Player.Black, PieceType.Checker), new Uri("ms-appx:///Assets/BlackChecker.png", UriKind.Absolute)},
                {Tuple.Create(Player.Black, PieceType.King), new Uri("ms-appx:///Assets/BlackKing.png", UriKind.Absolute)}
            };

        public MainPage()
        {
            InitializeComponent();
        }

        public void UpdateBoard(Board board)
        {
            var boardList = board.Board;

            for (var rowIndex = 0; rowIndex < boardList.Length; rowIndex++)
            {
                for (var colIndex = 0; colIndex < boardList[rowIndex].Length; colIndex++)
                {
                    var piece = boardList[rowIndex][colIndex];
                    if (piece == FSharpOption<Piece>.None)
                    {
                        continue;
                    }

                    var bitmapImage = new BitmapImage();
                    bitmapImage.UriSource = pieceToUriMap[Tuple.Create(piece.Value.Player, piece.Value.PieceType)];

                    var image = new Image();
                    image.Source = bitmapImage;
                    Grid.SetRow(image, rowIndex);
                    Grid.SetColumn(image, colIndex);
                    Board.Children.Add(image);
                }
            }
        }
    }
}
