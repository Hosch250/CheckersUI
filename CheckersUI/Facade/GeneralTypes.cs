﻿using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public enum Player
    {
        White, Black
    }

    public enum PieceType
    {
        Checker,
        King
    }

    public static class Extensions
    {
        public static Player Convert(this Checkers.Types.Player value) =>
            Equals(value, Checkers.Types.Player.Black) ? Player.Black : Player.White;

        public static Checkers.Types.Player ConvertBack(this Player value) =>
            value == Player.Black ? Checkers.Types.Player.Black : Checkers.Types.Player.White;

        public static PieceType Convert(this Checkers.Types.PieceType value) =>
            Equals(value, Checkers.Types.PieceType.Checker) ? PieceType.Checker : PieceType.King;

        public static Checkers.Types.PieceType ConvertBack(this PieceType value) =>
            value == PieceType.Checker ? Checkers.Types.PieceType.Checker : Checkers.Types.PieceType.King;

        public static Piece Convert(this FSharpOption<Checkers.Piece.Piece> piece)
        {
            if (Equals(piece, Checkers.Piece.whiteChecker))
            {
                return Piece.WhiteChecker;
            }
            if (Equals(piece, Checkers.Piece.whiteKing))
            {
                return Piece.WhiteKing;
            }
            if (Equals(piece, Checkers.Piece.blackChecker))
            {
                return Piece.BlackChecker;
            }
            if (Equals(piece, Checkers.Piece.blackKing))
            {
                return Piece.BlackKing;
            }

            return null;
        }

        public static FSharpOption<Checkers.Piece.Piece> ConvertBack(this Piece piece)
        {
            if (Equals(piece, Piece.WhiteChecker))
            {
                return Checkers.Piece.whiteChecker;
            }
            if (Equals(piece, Piece.WhiteKing))
            {
                return Checkers.Piece.whiteKing;
            }
            if (Equals(piece, Piece.BlackChecker))
            {
                return Checkers.Piece.blackChecker;
            }
            if (Equals(piece, Piece.BlackKing))
            {
                return Checkers.Piece.blackKing;
            }

            return null;
        }
    }
}