using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class BoardType
    {
        public List<List<Piece>> Board { get; }

        public BoardType(IEnumerable<IEnumerable<FSharpOption<Checkers.Piece.Piece>>> board)
        {
            Board = board.Select(row => row.Select(piece => piece.Convert()).ToList()).ToList();
        }

        public BoardType() : this(Checkers.Board.defaultBoard) { }

        public static implicit operator BoardType(FSharpList<FSharpList<FSharpOption<Checkers.Piece.Piece>>> value)
        {
            return new BoardType(value);
        }

        public static implicit operator FSharpList<FSharpList<FSharpOption<Checkers.Piece.Piece>>>(BoardType value)
        {
            return Checkers.Board.listFromSeq(value.Board.Select(row => row.Select(piece => piece.ConvertBack())));
        }
    }
}
