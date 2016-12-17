using Checkers;
using CheckersUI.Command;
using Microsoft.FSharp.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckersUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            Controller = GameController.newGame;
        }

        private string PlayerToString(Types.Player player) =>
            player.IsWhite ? nameof(Types.Player.White) : nameof(Types.Player.Black);
        
        private GameController.GameController _controller;
        public GameController.GameController Controller
        {
            get
            {
                return _controller;
            }
            set
            {
                _controller = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));

            }
        }

        private Types.Coord _selection;
        public Types.Coord Selection
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
                else if (_selection != null && PublicAPI.isValidMove(_selection, value, Controller))
                {
                    Controller = PublicAPI.move(_selection, value, Controller).Value;
                    _selection = null;
                }
                else if (Controller.Board[value.Row][value.Column] == FSharpOption<Piece.Piece>.None)
                {
                    _selection = null;
                }
                else
                {
                    _selection = value;
                }
            }
        }
        
        public string Status
        {
            get
            {
                var winningPlayer = PublicAPI.isWon(Controller);
                return FSharpOption<Types.Player>.get_IsSome(winningPlayer)
                       ? $"{PlayerToString(winningPlayer.Value)} Won!"
                       : $"{PlayerToString(Controller.Player)}'s turn";
            }
        }

        private DelegateCommand _newGameCommand;
        public DelegateCommand NewGameCommand
        {
            get
            {
                if (_newGameCommand != null)
                {
                    return _newGameCommand;
                }

                _newGameCommand = new DelegateCommand(sender => CreateNewGame());
                return _newGameCommand;
            }
        }

        internal void CreateNewGame() =>
            Controller = new GameController.GameController(Board.defaultBoard, Types.Player.Black);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}