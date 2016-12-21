﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class GameController : INotifyPropertyChanged
    {
        public GameController(Board board, Player currentPlayer, Coord currentCoord = null)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            CurrentCoord = currentCoord;
        }

        public Player CurrentPlayer { get; }
        public Coord CurrentCoord { get; }

        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                Debug.Assert(value.GameBoard != null);
                _board = value;
                OnPropertyChanged();
            }
        }

        public GameController(Checkers.GameController.GameController gameController)
            :this(gameController.Board, gameController.CurrentPlayer.Convert(), gameController.CurrentCoord) { }

        public GameController()
            : this(new Board(), Player.Black) { }

        public GameController Move(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.movePiece(startCoord, endCoord, this);

        public GameController Move(IEnumerable<Coord> moves) =>
            Checkers.PublicAPI.move(moves.Select(item => (Checkers.Types.Coord)item), this);

        public bool IsValidMove(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.isValidMove(startCoord, endCoord, this);

        public IEnumerable<Coord> GetMove() =>
            Checkers.PublicAPI.getMove(CurrentPlayer.ConvertBack(), 1, Board).Select(coord => (Coord)coord);

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
            return new Checkers.GameController.GameController(controller.Board, controller.CurrentPlayer.ConvertBack(),
                controller.CurrentCoord);
        }

        public static implicit operator GameController(FSharpOption<Checkers.GameController.GameController> controller)
        {
            return Equals(controller, FSharpOption<Checkers.GameController.GameController>.None)
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
