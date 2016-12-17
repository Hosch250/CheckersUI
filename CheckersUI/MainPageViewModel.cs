using CheckersUI.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckersUI.Facade;

namespace CheckersUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            Controller = new GameController();
        }
        
        private GameController _controller;
        public GameController Controller
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
                else if (_selection != null && Controller.IsValidMove(_selection, value))
                {
                    Controller = Controller.Move(_selection, value);
                    _selection = null;
                }
                else if (Controller.Board.GameBoard[value.Row][value.Column] == null)
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
                var winningPlayer = Controller.GetWinningPlayer();
                return winningPlayer.HasValue
                       ? $"{winningPlayer.Value} Won!"
                       : $"{Controller.CurrentPlayer}'s turn";
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
            Controller = new GameController();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}