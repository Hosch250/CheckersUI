using Checkers;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class Coord
    {
        public int Row { get; }
        public int Column { get; }

        public Coord(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public Coord Offset(Coord coord) =>
            new Coord(Row + coord.Row, Column + coord.Column);

        public static implicit operator Coord(Types.Coord coord)
        {
            return new Coord(coord.Row, coord.Column);
        }

        public static implicit operator Types.Coord(Coord coord)
        {
            return new Types.Coord(coord.Row, coord.Column);
        }

        public static implicit operator Coord(FSharpOption<Types.Coord> coord)
        {
            return Equals(coord, FSharpOption<Types.Coord>.None)
                ? null
                : new Coord(coord.Value.Row, coord.Value.Column);
        }

        public static implicit operator FSharpOption<Types.Coord>(Coord coord)
        {
            return coord == null
                ? FSharpOption<Types.Coord>.None
                : FSharpOption<Types.Coord>.Some(new Types.Coord(coord.Row, coord.Column));
        }
    }
}
