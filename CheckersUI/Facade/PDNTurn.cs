using Checkers;

namespace CheckersUI.Facade
{
    public class PDNTurn
    {
        public PDNTurn(int moveNumber, PDNMove blackMove, PDNMove whiteMove)
        {
            MoveNumber = moveNumber;
            BlackMove = blackMove;
            WhiteMove = whiteMove;
        }

        public int MoveNumber { get; }
        public PDNMove BlackMove { get; }
        public PDNMove WhiteMove { get; }

        public static implicit operator PDNTurn(Types.PDNTurn value)
        {
            return new PDNTurn(value.MoveNumber, value.BlackMove, value.WhiteMove);
        }

        public static implicit operator Types.PDNTurn(PDNTurn value)
        {
            return new Types.PDNTurn(value.MoveNumber, value.BlackMove, value.WhiteMove);
        }
    }
}
