using Checkers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersUI.Facade
{
    public class PdnMove
    {
        public PdnMove(List<int> move, string resultingFen, string displayString)
        {
            Move = move;
            ResultingFen = resultingFen;
            DisplayString = displayString;
        }

        public List<int> Move { get; }
        public string ResultingFen { get; }
        public string DisplayString { get; }

        public bool IsJump()
        {
            var firstCoord = (Coord)PublicAPI.pdnBoardCoords(Variant.AmericanCheckers.ToGameVariant().pdnMembers)[Move[0]];
            var secondCoord = (Coord)PublicAPI.pdnBoardCoords(Variant.AmericanCheckers.ToGameVariant().pdnMembers)[Move[1]];

            return Math.Abs(firstCoord.Row - secondCoord.Row) == 2;
        }

        public static implicit operator PdnMove(Generic.PdnMove value)
        {
            return new PdnMove(value.Move.ToList(), value.ResultingFen, value.DisplayString);
        }

        public static implicit operator Generic.PdnMove(PdnMove value)
        {
            return new Generic.PdnMove(Generic.listFromSeq(value.Move).Value, value.ResultingFen, value.DisplayString);
        }

        public static implicit operator PdnMove(FSharpOption<Generic.PdnMove> value)
        {
            return Equals(value, FSharpOption<Generic.PdnMove>.None)
                ? null
                : new PdnMove(value.Value.Move.ToList(), value.Value.ResultingFen, value.Value.DisplayString);
        }

        public static implicit operator FSharpOption<Generic.PdnMove>(PdnMove value)
        {
            return value == null
                ? FSharpOption<Generic.PdnMove>.None
                : new FSharpOption<Generic.PdnMove>(new Generic.PdnMove(Generic.listFromSeq(value.Move).Value, value.ResultingFen, value.DisplayString));
        }
    }
}
