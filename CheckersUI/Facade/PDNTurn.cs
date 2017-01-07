using Checkers;

namespace CheckersUI.Facade
{
    public class PdnTurn
    {
        public PdnTurn(int moveNumber, PdnMove blackMove, PdnMove whiteMove)
        {
            MoveNumber = moveNumber;
            BlackMove = blackMove;
            WhiteMove = whiteMove;
        }

        public int MoveNumber { get; }
        public PdnMove BlackMove { get; }
        public PdnMove WhiteMove { get; }

        public static implicit operator PdnTurn(Types.PDNTurn value)
        {
            return new PdnTurn(value.MoveNumber, value.BlackMove, value.WhiteMove);
        }

        public static implicit operator Types.PDNTurn(PdnTurn value)
        {
            return new Types.PDNTurn(value.MoveNumber, value.BlackMove, value.WhiteMove);
        }
    }
}
