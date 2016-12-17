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

        public static Coord operator +(Coord coord1, Coord cooord2)
        {
            return new Coord(coord1.Row + cooord2.Row, coord1.Column + cooord2.Column);
        }

        public static implicit operator Coord(Checkers.Types.Coord coord)
        {
            return new Coord(coord.Row, coord.Column);
        }

        public static implicit operator Checkers.Types.Coord(Coord coord)
        {
            return new Checkers.Types.Coord(coord.Row, coord.Column);
        }

        public static implicit operator Coord(FSharpOption<Checkers.Types.Coord> coord)
        {
            return Equals(coord, FSharpOption<Checkers.Types.Coord>.None)
                ? null
                : new Coord(coord.Value.Row, coord.Value.Column);
        }

        public static implicit operator FSharpOption<Checkers.Types.Coord>(Coord coord)
        {
            return coord == null
                ? FSharpOption<Checkers.Types.Coord>.None
                : FSharpOption<Checkers.Types.Coord>.Some(new Checkers.Types.Coord(coord.Row, coord.Column));
        }
    }
}
