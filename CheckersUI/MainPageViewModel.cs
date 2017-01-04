using CheckersUI.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckersUI.Facade;
using Windows.Storage;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace CheckersUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        public MainPageViewModel()
        {
            Controller = new GameController();

            IsGameInProgress = true;
            BlackOpponent = Opponent.Human;
            WhiteOpponent = Opponent.Computer;
            Level = 8;

            PlayerTurn += HandlePlayerTurnAsync;

            var tmpTheme = (string)_roamingSettings.Values["Theme"];
            SelectedTheme = string.IsNullOrEmpty(tmpTheme) ? Theme.Wood : (Theme)Enum.Parse(typeof(Theme), tmpTheme);

            var tmpEnableSoundEffects = (string)_roamingSettings.Values["EnableSoundEffects"];
            EnableSoundEffects = string.IsNullOrEmpty(tmpEnableSoundEffects) || bool.Parse(tmpEnableSoundEffects);
        }

        private static CancellationTokenSource _cancelComputerMoveTokenSource;
        private async void HandlePlayerTurnAsync(object sender, Player e)
        {
            if ((e == Player.Black && BlackOpponent == Opponent.Computer ||
                e == Player.White && WhiteOpponent == Opponent.Computer) &&
                e == Controller.CurrentPlayer &&
                Controller.GetWinningPlayer() == null)
            {
                _cancelComputerMoveTokenSource?.Dispose();
                _cancelComputerMoveTokenSource = new CancellationTokenSource();

                List<Coord> move;
                await Task.Run(async () =>
                {
                    move = Controller.GetMove(Level).ToList();
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        MovePiece(move));
                }, _cancelComputerMoveTokenSource.Token);

                if (!_cancelComputerMoveTokenSource.IsCancellationRequested)
                {
                    OnPlayerTurn(OtherPlayer(e));
                }
            }
        }

        private static Player OtherPlayer(Player player) =>
            player == Player.Black ? Player.White : Player.Black;

        private GameController _controller;
        public GameController Controller
        {
            get { return _controller; }
            set
            {
                _controller = value;

                var winningPlayer = Controller.GetWinningPlayer();
                IsGameInProgress = !(winningPlayer.HasValue && winningPlayer.Value == Controller.CurrentPlayer);

                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(CanTakeback));
                OnPropertyChanged(nameof(MoveHistory));
            }
        }

        private async void PlayEffectAsync()
        {
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync($"Assets\\{SelectedTheme}Theme");
            var file = await folder.GetFileAsync("CheckerClick.mp3");
            var stream = await file.OpenAsync(FileAccessMode.Read);

            var player = BackgroundMediaPlayer.Current;
            var mediaSource = MediaSource.CreateFromStream(stream, file.ContentType);
            (player.Source as IDisposable)?.Dispose();
            player.Source = mediaSource;
            
            player.Play();
        }

        private void MovePiece(List<Coord> move)
        {
            if (!IsGameInProgress)
            {
                return;
            }

            if (EnableSoundEffects)
            {
                PlayEffectAsync();
            }

            _selection = move.Last();
            Controller = Controller.Move(move);
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
                if (Controller.CurrentPlayer == Player.Black && BlackOpponent == Opponent.Computer ||
                    Controller.CurrentPlayer == Player.White && WhiteOpponent == Opponent.Computer)
                {
                    return;
                }

                if (_selection != null && Controller.IsValidMove(_selection, value))
                {
                    MovePiece(new List<Coord> {_selection, value});

                    var piece = Controller.Board[_selection];
                    OnPlayerTurn(OtherPlayer(piece.Player));
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

        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get
            {
                var winningPlayer = Controller.GetWinningPlayer();
                return winningPlayer.HasValue && winningPlayer.Value != Controller.CurrentPlayer
                       ? $"{winningPlayer.Value} Won!"
                       : $"{Controller.CurrentPlayer}'s turn";
            }
        }

        private bool _displaySettingsGrid;
        public bool DisplaySettingsGrid
        {
            get { return _displaySettingsGrid; }
            set
            {
                _displaySettingsGrid = value;
                OnPropertyChanged();
            }
        }

        private bool _displayCreateGameGrid;
        public bool DisplayCreateGameGrid
        {
            get { return _displayCreateGameGrid; }
            set
            {
                _displayCreateGameGrid = value;
                OnPropertyChanged();
            }
        }

        private Theme _selectedTheme;
        public Theme SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                }

                AssignRoamingSetting("Theme", value.ToString());
            }
        }

        private bool _enableSoundEffects;
        public bool EnableSoundEffects
        {
            get { return _enableSoundEffects; }
            set
            {
                if (_enableSoundEffects != value)
                {
                    _enableSoundEffects = value;
                    OnPropertyChanged();
                }

                AssignRoamingSetting("EnableSoundEffects", value.ToString());
            }
        }
        
        public Player BoardOrientation =>
            BlackOpponent == Opponent.Human ? Player.Black : Player.White;

        private void AssignRoamingSetting(string name, string value)
        {
            if ((string)_roamingSettings.Values[name] != value)
            {
                _roamingSettings.Values[name] = value;
                ApplicationData.Current.SignalDataChanged();
            }
        }

        public List<Theme> Themes =>
            Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();

        public List<Opponent> Opponents =>
            Enum.GetValues(typeof(Opponent)).Cast<Opponent>().ToList();

        private string GetOpponentText(Opponent opponent) =>
            opponent == Opponent.Human
            ? opponent.ToString()
            : opponent.ToString() + " Level " + Level;

        private Opponent _whiteOpponent;
        public Opponent WhiteOpponent
        {
            get { return _whiteOpponent; }
            set
            {
                if (_whiteOpponent != value)
                {
                    _whiteOpponent = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WhiteOpponentText =>
            GetOpponentText(WhiteOpponent);

        private Opponent _blackOpponent;
        public Opponent BlackOpponent
        {
            get { return _blackOpponent; }
            set
            {
                if (_blackOpponent != value)
                {
                    _blackOpponent = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BlackOpponentText =>
            GetOpponentText(BlackOpponent);

        public IEnumerable<string> MoveHistory =>
            Controller.MoveHistory.Select(m => $"{m.MoveNumber}:\t{m.BlackMove.DisplayString},\t{m.WhiteMove?.DisplayString}");

        private bool _isGameInProgress;
        public bool IsGameInProgress
        {
            get { return _isGameInProgress; }
            set
            {
                if (_isGameInProgress != value)
                {
                    _isGameInProgress = value;
                    OnPropertyChanged();

                    if (!IsGameInProgress)
                    {
                        _cancelComputerMoveTokenSource?.Cancel();
                    }
                }
            }
        }
        
        public bool CanTakeback
        {
            get
            {
                return Controller.MoveHistory.Any() &&
                       Controller.GetWinningPlayer() == null &&
                       (BlackOpponent == Opponent.Human || WhiteOpponent == Opponent.Human);
            }
        }

        private DelegateCommand _toggleDisplaySettingsCommand;
        public DelegateCommand ToggleDisplaySettingsCommand
        {
            get
            {
                if (_toggleDisplaySettingsCommand != null)
                {
                    return _toggleDisplaySettingsCommand;
                }

                _toggleDisplaySettingsCommand = new DelegateCommand(sender => DisplaySettingsGrid = !DisplaySettingsGrid);
                return _toggleDisplaySettingsCommand;
            }
        }

        private DelegateCommand _displayCreateGameCommand;
        public DelegateCommand DisplayCreateGameCommand
        {
            get
            {
                if (_displayCreateGameCommand != null)
                {
                    return _displayCreateGameCommand;
                }

                _displayCreateGameCommand = new DelegateCommand(sender => DisplayCreateGameGrid = true);
                return _displayCreateGameCommand;
            }
        }

        private DelegateCommand _createGameCommand;
        public DelegateCommand CreateGameCommand
        {
            get
            {
                if (_createGameCommand != null)
                {
                    return _createGameCommand;
                }
                
                _createGameCommand = new DelegateCommand(sender => CreateGame());
                return _createGameCommand;
            }
        }

        private DelegateCommand _undoMoveCommand;
        public DelegateCommand UndoMoveCommand
        {
            get
            {
                if (_undoMoveCommand != null)
                {
                    return _undoMoveCommand;
                }

                _undoMoveCommand = new DelegateCommand(sender => TakebackMove());
                return _undoMoveCommand;
            }
        }

        private DelegateCommand _cancelGameCommand;
        public DelegateCommand CancelGameCommand
        {
            get
            {
                if (_cancelGameCommand != null)
                {
                    return _cancelGameCommand;
                }

                _cancelGameCommand = new DelegateCommand(sender => IsGameInProgress = false);
                return _cancelGameCommand;
            }
        }

        private void CreateGame()
        {
            DisplayCreateGameGrid = false;
            IsGameInProgress = true;
            Controller = new GameController();

            OnPropertyChanged(nameof(BoardOrientation));
            OnPropertyChanged(nameof(BlackOpponentText));
            OnPropertyChanged(nameof(WhiteOpponentText));

            OnPlayerTurn(Player.Black);
        }

        private void TakebackMove()
        {
            Controller = Controller.TakebackMove();

            if (Controller.CurrentPlayer == Player.Black && BlackOpponent == Opponent.Computer ||
                Controller.CurrentPlayer == Player.White && WhiteOpponent == Opponent.Computer)
            {
                Controller = Controller.TakebackMove();
            }

            OnPlayerTurn(Controller.CurrentPlayer);
        }

        public event EventHandler<Player> PlayerTurn;
        protected virtual void OnPlayerTurn(Player e) =>
            PlayerTurn?.Invoke(this, e);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}