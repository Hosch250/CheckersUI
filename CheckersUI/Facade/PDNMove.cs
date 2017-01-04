using Checkers;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckersUI.Facade
{
    public class PDNMove
    {
        public PDNMove(List<int> move, string resultingFen, string displayString)
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
            var firstCoord = (Coord)PublicAPI.getPdnCoord(Move[0]);
            var secondCoord = (Coord)PublicAPI.getPdnCoord(Move[1]);

            return Math.Abs(firstCoord.Row - secondCoord.Row) == 2;
        }

        public static implicit operator PDNMove(Checkers.Types.PDNMove value)
        {
            return new PDNMove(value.Move.ToList(), value.ResultingFen, value.DisplayString);
        }

        public static implicit operator Types.PDNMove(PDNMove value)
        {
            return new Types.PDNMove(Types.listFromSeq(value.Move).Value, value.ResultingFen, value.DisplayString);
        }

        public static implicit operator PDNMove(FSharpOption<Types.PDNMove> value)
        {
            return Equals(value, FSharpOption<Types.PDNMove>.None)
                ? null
                : new PDNMove(value.Value.Move.ToList(), value.Value.ResultingFen, value.Value.DisplayString);
        }

        public static implicit operator FSharpOption<Types.PDNMove>(PDNMove value)
        {
            return value == null
                ? FSharpOption<Types.PDNMove>.None
                : new FSharpOption<Types.PDNMove>(new Types.PDNMove(Types.listFromSeq(value.Move).Value, value.ResultingFen, value.DisplayString));
        }
    }
}
