using Checkers;
using Microsoft.FSharp.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckersUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private MainPage _page;

        public MainPageViewModel(MainPage page)
        {
            _page = page;
            GameController = new GameController.GameController(Board.defaultBoard, Types.Player.White);
        }

        private string PlayerToString(Types.Player player) =>
            player.IsWhite ? nameof(Types.Player.White) : nameof(Types.Player.Black);
        
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
                else if (_selection != null && PublicAPI.isValidMove(_selection, value, GameController))
                {
                    GameController = PublicAPI.move(_selection, value, GameController).Value;
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
        
        public string Status
        {
            get
            {
                var winningPlayer = PublicAPI.isWon(GameController);
                return FSharpOption<Types.Player>.get_IsSome(winningPlayer)
                       ? $"{PlayerToString(winningPlayer.Value)} Won!"
                       : $"{PlayerToString(GameController.Player)}'s turn";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}