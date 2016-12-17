using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class GameController
    {
        public readonly Board Board;
        public readonly Player CurrentPlayer;
        public readonly Coord CurrentCoord;

        public GameController(Board board, Player currentPlayer, Coord currentCoord = null)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            CurrentCoord = currentCoord;
        }

        public GameController(Checkers.GameController.GameController gameController)
            :this(gameController.Board, gameController.CurrentPlayer.Convert(), gameController.CurrentCoord) { }

        public GameController()
            : this(new Board(), Player.Black) { }

        public GameController Move(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.move(startCoord, endCoord, this);

        public bool IsValidMove(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.isValidMove(startCoord, endCoord, this);

        public Player? GetWinningPlayer()
        {
            var player = Checkers.PublicAPI.isWon(this);
            return player == FSharpOption<Checkers.Types.Player>.None ? new Player?() : player.Value.Convert();
        }

        public static implicit operator GameController(Checkers.GameController.GameController controller)
        {
            return new GameController(controller);
        }

        public static implicit operator Checkers.GameController.GameController(GameController controller)
        {
            return new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(),
                controller.CurrentCoord);
        }

        public static implicit operator GameController(FSharpOption<Checkers.GameController.GameController> controller)
        {
            return controller == FSharpOption<Checkers.GameController.GameController>.None
                ? null
                : new GameController(controller.Value);
        }

        public static implicit operator FSharpOption<Checkers.GameController.GameController>(GameController controller)
        {
            return controller == null
                ? FSharpOption<Checkers.GameController.GameController>.None
                : FSharpOption<Checkers.GameController.GameController>.Some(
                    new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(),
                        controller.CurrentCoord));
        }
    }
}
