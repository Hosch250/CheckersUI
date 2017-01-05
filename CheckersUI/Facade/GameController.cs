using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class GameController : INotifyPropertyChanged
    {
        public GameController(Board board, Player currentPlayer, List<PDNTurn> moveHistory, Coord currentCoord = null)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            MoveHistory = moveHistory;
            CurrentCoord = currentCoord;
        }

        public GameController(Checkers.GameController.GameController gameController)
            : this(gameController.Board, gameController.CurrentPlayer.Convert(), gameController.MoveHistory.Select(item => (PDNTurn)item).ToList(), gameController.CurrentCoord) { }

        public GameController()
            : this(new Board(), Player.Black, new List<PDNTurn>()) { }

        public static GameController FromPosition(string fenPosition)
        {
            try
            {
                return Checkers.PortableDraughtsNotation.controllerFromFen(fenPosition);
            }
            catch
            {
                // invalid fen entered
                // todo: notify the user
                return null;
            }
        }

        public static bool TryFromPosition(string fenPosition, out GameController controller)
        {
            try
            {
                controller = Checkers.PortableDraughtsNotation.controllerFromFen(fenPosition);
                return true;
            }
            catch
            {
                controller = new GameController();
                return false;
            }
        }

        public Player CurrentPlayer { get; }
        public Coord CurrentCoord { get; }

        private List<PDNTurn> _moveHistory;
        public List<PDNTurn> MoveHistory
        {
            get { return _moveHistory; }
            set
            {
                _moveHistory = value;
                OnPropertyChanged();
            }
        }

        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                _board = value;
                OnPropertyChanged();
            }
        }

        public GameController Move(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.movePiece(startCoord, endCoord, this);

        public GameController Move(IEnumerable<Coord> moves) =>
            Checkers.PublicAPI.move(moves.Select(item => (Checkers.Types.Coord)item), this);

        public bool IsValidMove(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.isValidMove(startCoord, endCoord, this);

        public IEnumerable<Coord> GetMove(int searchDepth) =>
            Checkers.PublicAPI.getMove(searchDepth, this).Select(coord => (Coord)coord);

        public GameController TakebackMove() =>
            Checkers.PublicAPI.takeBackMove(this);

        public Player? GetWinningPlayer()
        {
            var player = Checkers.PublicAPI.isWon(this);
            return Equals(player, FSharpOption<Checkers.Types.Player>.None) ? new Player?() : player.Value.Convert();
        }

        public static implicit operator GameController(Checkers.GameController.GameController controller)
        {
            return new GameController(controller);
        }

        public static implicit operator Checkers.GameController.GameController(GameController controller)
        {
            var moveHistory = Checkers.Types.listFromSeq(controller.MoveHistory.Select(item => (Checkers.Types.PDNTurn)item)).Value;

            return new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(),
                controller.CurrentCoord, moveHistory);
        }

        public static implicit operator GameController(FSharpOption<Checkers.GameController.GameController> controller)
        {
            return Equals(controller, FSharpOption<Checkers.GameController.GameController>.None)
                ? null
                : new GameController(controller.Value);
        }

        public static implicit operator FSharpOption<Checkers.GameController.GameController>(GameController controller)
        {
            var moveHistory = Checkers.Types.listFromSeq(controller.MoveHistory.Select(item => (Checkers.Types.PDNTurn)item)).Value;

            return controller == null
                ? FSharpOption<Checkers.GameController.GameController>.None
                : FSharpOption<Checkers.GameController.GameController>.Some(
                    new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(),
                        controller.CurrentCoord, moveHistory));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
