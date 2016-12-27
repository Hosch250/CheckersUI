using CheckersUI.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckersUI.Facade;
using Windows.Storage;
using System.Collections.Generic;
using System;
using System.Linq;
using Windows.Media.Playback;
using Windows.Media.Core;
using System.Threading.Tasks;

namespace CheckersUI
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        public MainPageViewModel()
        {
            Controller = new GameController();

            BlackOpponent = Opponent.Human;
            WhiteOpponent = Opponent.Computer;

            PlayerTurn += HandlePlayerTurn;

            var tmpTheme = (string)_roamingSettings.Values["Theme"];
            SelectedTheme = string.IsNullOrEmpty(tmpTheme) ? Theme.Wood : (Theme)Enum.Parse(typeof(Theme), tmpTheme);

            var tmpEnableSoundEffects = (string)_roamingSettings.Values["EnableSoundEffects"];
            EnableSoundEffects = string.IsNullOrEmpty(tmpEnableSoundEffects) || bool.Parse(tmpEnableSoundEffects);
        }

        private void HandlePlayerTurn(object sender, Player e)
        {
            if (e == Player.Black && BlackOpponent == Opponent.Computer ||
                e == Player.White && WhiteOpponent == Opponent.Computer)
            {
                var move = Controller.GetMove(7).ToList();
                MovePiece(move);

                OnPlayerTurn(OtherPlayer(e));
            }
        }

        private Player OtherPlayer(Player player) =>
            player == Player.Black ? Player.White : Player.Black;

        private GameController _controller;
        public GameController Controller
        {
            get { return _controller; }
            set
            {
                _controller = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
            }
        }

        private async Task PlayEffectAsync()
        {
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync($"Assets\\{SelectedTheme.ToString()}Theme");
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
            if (EnableSoundEffects)
            {
                Task.Run(PlayEffectAsync).ContinueWith(s =>
                {
                    Controller = Controller.Move(move);
                    _selection = move.Last();
                });
            }
            else
            {
                Controller = Controller.Move(move);
                _selection = move.Last();
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
                if (_selection != null && Controller.IsValidMove(_selection, value))
                {
                    var piece = Controller.Board[_selection];
                    MovePiece(new List<Coord> {_selection, value});
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

        public DelegateCommand DisplaySettingsCommand =>
            new DelegateCommand(sender => DisplaySettingsGrid = true);
        
        public DelegateCommand HideSettingsCommand =>
            new DelegateCommand(sender => DisplaySettingsGrid = false);

        public DelegateCommand DisplayCreateGameCommand =>
            new DelegateCommand(sender => DisplayCreateGameGrid = true);

        public DelegateCommand CreateGameCommand =>
            new DelegateCommand(sender =>
            {
                DisplayCreateGameGrid = false;
                Controller = new GameController();
                OnPlayerTurn(Player.Black);
            });

        public event EventHandler<Player> PlayerTurn;
        protected virtual void OnPlayerTurn(Player e) =>
            PlayerTurn?.Invoke(this, e);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}