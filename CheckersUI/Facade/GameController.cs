using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class GameController : INotifyPropertyChanged
    {
        public GameController(Board board, Player currentPlayer, string initialPosition, List<PdnTurn> moveHistory, Coord currentCoord = null)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            InitialPosition = initialPosition;
            MoveHistory = moveHistory;
            CurrentCoord = currentCoord;

            Fen = Checkers.PortableDraughtsNotation.createFen(currentPlayer.ConvertBack(), board);
        }

        public GameController(Checkers.GameController.GameController gameController)
            : this(gameController.Board, gameController.CurrentPlayer.Convert(), gameController.InitialPosition, gameController.MoveHistory.Select(item => (PdnTurn)item).ToList(), gameController.CurrentCoord) { }

        public GameController()
            : this(new Board(), Player.Black, Checkers.PortableDraughtsNotation.createFen(Player.Black.ConvertBack(), new Board()), new List<PdnTurn>()) { }

        public GameController WithBoard(string fen) =>
            new GameController(FromPosition(fen).Board, CurrentPlayer, InitialPosition, MoveHistory, CurrentCoord);
        
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
        public string InitialPosition { get; }
        public string Fen { get; }

        private List<PdnTurn> _moveHistory;
        public List<PdnTurn> MoveHistory
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
            Checkers.PublicAPI.move(moves.Select(item => (Checkers.Generic.Coord)item), this);

        public bool IsValidMove(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.isValidMove(startCoord, endCoord, this);

        public IEnumerable<Coord> GetMove(int searchDepth) =>
            Checkers.PublicAPI.getMove(searchDepth, this).Select(coord => (Coord)coord);

        public GameController TakebackMove() =>
            Checkers.PublicAPI.takeBackMove(this);

        public Player? GetWinningPlayer()
        {
            var player = Checkers.PublicAPI.isWon(this);
            return Equals(player, FSharpOption<Checkers.Generic.Player>.None) ? new Player?() : player.Value.Convert();
        }

        public static implicit operator GameController(Checkers.GameController.GameController controller)
        {
            return new GameController(controller);
        }

        public static implicit operator Checkers.GameController.GameController(GameController controller)
        {
            var moveHistory = Checkers.Generic.listFromSeq(controller.MoveHistory.Select(item => (Checkers.Generic.PdnTurn)item)).Value;

            return new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(), controller.InitialPosition, moveHistory, controller.CurrentCoord);
        }

        public static implicit operator GameController(FSharpOption<Checkers.GameController.GameController> controller)
        {
            return Equals(controller, FSharpOption<Checkers.GameController.GameController>.None)
                ? null
                : new GameController(controller.Value);
        }

        public static implicit operator FSharpOption<Checkers.GameController.GameController>(GameController controller)
        {
            var moveHistory = Checkers.Generic.listFromSeq(controller.MoveHistory.Select(item => (Checkers.Generic.PdnTurn)item)).Value;

            return controller == null
                ? FSharpOption<Checkers.GameController.GameController>.None
                : FSharpOption<Checkers.GameController.GameController>.Some(
                    new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(), controller.InitialPosition, moveHistory, controller.CurrentCoord));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
