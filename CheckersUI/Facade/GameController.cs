﻿using System;
using Microsoft.FSharp.Core;

namespace CheckersUI.Facade
{
    public class GameController
    {
        public readonly BoardType Board;
        public readonly Player CurrentPlayer;
        public readonly Coord CurrentCoord;

        public GameController(Checkers.GameController.GameController gameController)
        {
            Board = gameController.Board;
            CurrentPlayer = gameController.CurrentPlayer.Convert();
            CurrentCoord = gameController.CurrentCoord;
        }

        public GameController()
        {
            Board = new BoardType();
            CurrentPlayer = Player.Black;
            CurrentCoord = null;
        }

        public GameController Move(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.move(startCoord, endCoord, this);

        public bool IsValidMove(Coord startCoord, Coord endCoord) =>
            Checkers.PublicAPI.isValidMove(startCoord, endCoord, this);

        public Player? WonBy()
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
