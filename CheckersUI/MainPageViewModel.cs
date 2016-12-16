using Checkers;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Collections.Generic;
using static Checkers.Types;
using static Checkers.PublicAPI;

namespace CheckersUI
{
    public class MainPageViewModel
    {
        private MainPage _page;

        public MainPageViewModel(MainPage page)
        {
            _page = page;
            GameController = new GameController.GameController(Board.defaultBoard, Player.White);
        }

        private GameController.GameController _gameController;
        public GameController.GameController GameController
        {
            get
            {
                return _gameController;
            }
            set
            {
                _gameController = value;
                _page.UpdateBoard(value);
            }
        }

        private Coord _selection;
        public Coord Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (_selection == null)
                {
                    _selection = value;
                }
                else if (_selection != null && isValidMove(_selection, value, GameController))
                {
                    GameController = move(_selection, value, GameController).Value;
                    _selection = null;
                }
                else if (GameController.Board[value.Row][value.Column] == FSharpOption<Piece.Piece>.None)
                {
                    _selection = null;
                }
                else
                {
                    _selection = value;
                }
            }
        }
    }
}