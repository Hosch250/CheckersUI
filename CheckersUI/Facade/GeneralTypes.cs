﻿using Checkers;
using Microsoft.FSharp.Core;

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
        public static Player Convert(this Generic.Player value) =>
            Equals(value, Generic.Player.Black) ? Player.Black : Player.White;

        public static Generic.Player ConvertBack(this Player value) =>
            value == Player.Black ? Generic.Player.Black : Generic.Player.White;

        public static PieceType Convert(this Generic.PieceType value) =>
            Equals(value, Generic.PieceType.Checker) ? PieceType.Checker : PieceType.King;

        public static Generic.PieceType ConvertBack(this PieceType value) =>
            value == PieceType.Checker ? Generic.PieceType.Checker : Generic.PieceType.King;

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
