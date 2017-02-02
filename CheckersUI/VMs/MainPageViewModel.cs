using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using CheckersUI.Command;
using CheckersUI.Pages;

namespace CheckersUI.VMs
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public SmallGamePage SmallGamePage { get; }
        public GamePage GamePage { get; }
        public SmallBoardEditor SmallBoardEditor { get; }
        public BoardEditor BoardEditor { get; }

        private IPropertySet RoamingSettings
        {
            get
            {
                try
                {
                    return ApplicationData.Current.RoamingSettings.Values;
                }
                catch (InvalidOperationException)
                {
                    // we are running in a test and can't load the settings
                    return new PropertySet
                    {
                        {"Theme", "Wood"},
                        {"EnableSoundEffects", bool.FalseString},
                    };
                }
            }
        }

        public MainPageViewModel(SmallGamePage smallGamePage, GamePage gamePage, SmallBoardEditor smallBoardEditor, BoardEditor boardEditor)
        {
            SmallGamePage = smallGamePage;
            GamePage = gamePage;
            SmallBoardEditor = smallBoardEditor;
            BoardEditor = boardEditor;

            var tmpTheme = (string)RoamingSettings["Theme"];
            SelectedTheme = string.IsNullOrEmpty(tmpTheme) ? Theme.Wood : (Theme)Enum.Parse(typeof(Theme), tmpTheme);

            var tmpEnableSoundEffects = (string)RoamingSettings["EnableSoundEffects"];
            EnableSoundEffects = string.IsNullOrEmpty(tmpEnableSoundEffects) || bool.Parse(tmpEnableSoundEffects);
        }

        private void AssignRoamingSetting(string name, string value)
        {
            try
            {
                var roamingSettings = ApplicationData.Current.RoamingSettings;
                if ((string)roamingSettings.Values[name] != value)
                {
                    roamingSettings.Values[name] = value;
                    ApplicationData.Current.SignalDataChanged();
                }
            }
            catch (InvalidOperationException)
            {
                // we are running from a test, and can't load the settings
            }
        }

        public List<Theme> Themes =>
            Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();

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

        private DelegateCommand _gamePageNavigationCommand;
        public DelegateCommand GamePageNavigationCommand
        {
            get
            {
                if (_gamePageNavigationCommand != null)
                {
                    return _gamePageNavigationCommand;
                }

                _gamePageNavigationCommand = new DelegateCommand(param => OnNavigate("Game Page"));
                return _gamePageNavigationCommand;
            }
        }

        private DelegateCommand _boardEditorNavigationCommand;
        public DelegateCommand BoardEditorNavigationCommand
        {
            get
            {
                if (_boardEditorNavigationCommand != null)
                {
                    return _boardEditorNavigationCommand;
                }

                _boardEditorNavigationCommand = new DelegateCommand(param => OnNavigate("Board Editor"));
                return _boardEditorNavigationCommand;
            }
        }

        public event EventHandler<string> Navigate;
        protected virtual void OnNavigate(string pageName) =>
            Navigate?.Invoke(this, pageName);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}