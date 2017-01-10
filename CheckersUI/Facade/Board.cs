﻿using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class Board
    {
        public Piece[,] GameBoard { get; }

        public Board(FSharpOption<Checkers.Piece.Piece>[,] board)
        {
            var value = new Piece[8, 8];
            for (var row = 0; row < 8; row++)
            {
                for (var col = 0; col < 8; col++)
                {
                    value[row, col] = board[row, col].Convert();
                }
            }

            GameBoard = value;
        }

        public Board() : this(Checkers.Board.defaultBoard) { }

        public static Board EmptyBoard() =>
            new Board(Checkers.Board.emptyBoardList());

        public Piece this[Coord coord] => GameBoard[coord.Row, coord.Column];

        public static implicit operator Board(FSharpOption<Checkers.Piece.Piece>[,] value)
        {
            return new Board(value);
        }

        public static implicit operator FSharpOption<Checkers.Piece.Piece>[,](Board value)
        {
            var board = new FSharpOption<Checkers.Piece.Piece>[8, 8];
            for (var row = 0; row < 8; row++)
            {
                for (var col = 0; col < 8; col++)
                {
                    board[row, col] = value.GameBoard[row, col].ConvertBack();
                }
            }

            return board;
        }
    }
}
