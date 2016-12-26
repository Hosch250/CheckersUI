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
        private ApplicationDataContainer _roamingSettings = ApplicationData.Current.RoamingSettings;

        public MainPageViewModel()
        {
            Controller = new GameController();

            var tmpTheme = (string)_roamingSettings.Values["Theme"];
            SelectedTheme = string.IsNullOrEmpty(tmpTheme) ? Theme.Wood : (Theme)Enum.Parse(typeof(Theme), tmpTheme);
        }

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

        private async void MovePieceAsync(Coord startCoord, Coord endCoord)
        {
            await PlayEffectAsync();

            Controller = Controller.Move(startCoord, endCoord);
            _selection = endCoord;

            /*if (Controller.CurrentPlayer == Player.White && Controller.GetWinningPlayer() == null)
            {
                var move = Controller.GetMove(6);
                await PlayEffectAsync();
                Controller = Controller.Move(move);
            }*/
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
                    MovePieceAsync(_selection, value);
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

                if ((string)_roamingSettings.Values["Theme"] != value.ToString())
                {
                    _roamingSettings.Values["Theme"] = value.ToString();
                    ApplicationData.Current.SignalDataChanged();
                }
            }
        }

        public List<Theme> Themes =>
            Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();

        private DelegateCommand _displaySettingsCommand;
        public DelegateCommand DisplaySettingsCommand
        {
            get
            {
                if (_displaySettingsCommand != null)
                {
                    return _displaySettingsCommand;
                }

                _displaySettingsCommand = new DelegateCommand(sender => DisplaySettingsGrid = true);
                return _displaySettingsCommand;
            }
        }

        private DelegateCommand _hideSettingsCommand;
        public DelegateCommand HideSettingsCommand
        {
            get
            {
                if (_hideSettingsCommand != null)
                {
                    return _hideSettingsCommand;
                }

                _hideSettingsCommand = new DelegateCommand(sender => DisplaySettingsGrid = false);
                return _hideSettingsCommand;
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