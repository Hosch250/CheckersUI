using Checkers;
using Microsoft.FSharp.Core;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Checkers.Types;
using static Checkers.PublicAPI;

namespace CheckersUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private MainPage _page;

        public MainPageViewModel(MainPage page)
        {
            _page = page;
            GameController = new GameController.GameController(Board.defaultBoard, Player.White);
        }

        private string PlayerToString(Player player) =>
            player.IsWhite ? nameof(Player.White) : nameof(Player.Black);
        
        public IEnumerable<IEnumerable<FSharpOption<Piece.Piece>>> GameBoard =>
            _gameController.Board.Select(row => row.ToList());

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
                OnPropertyChanged();
                OnPropertyChanged(nameof(GameBoard));

                var winningPlayer = isWon(value);
                Status = FSharpOption<Player>.get_IsSome(winningPlayer)
                         ? $"{PlayerToString(winningPlayer.Value)} Won!"
                         : $"{PlayerToString(value.Player)}'s turn";
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

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}