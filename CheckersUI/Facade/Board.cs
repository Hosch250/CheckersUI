using Checkers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class Board
    {
        public List<List<Piece>> GameBoard { get; }

        public Board(IEnumerable<IEnumerable<FSharpOption<Checkers.Piece.Piece>>> board)
        {
            GameBoard = board.Select(row => row.Select(piece => piece.Convert()).ToList()).ToList();
        }

        public Board() : this(Checkers.Board.defaultBoard) { }

        public Piece this[Coord coord] => GameBoard[coord.Row][coord.Column];

        public static implicit operator Board(FSharpList<FSharpList<FSharpOption<Checkers.Piece.Piece>>> value)
        {
            return new Board(value);
        }

        public static implicit operator FSharpList<FSharpList<FSharpOption<Checkers.Piece.Piece>>>(Board value)
        {
            return Types.nestedListFromSeq(value.GameBoard.Select(row => row.Select(piece => piece.ConvertBack())));
        }
    }
}
