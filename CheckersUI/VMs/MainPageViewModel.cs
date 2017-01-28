using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using CheckersUI.Command;
using CheckersUI.Pages;

namespace CheckersUI.VMs
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public SmallGamePage SmallGamePage { get; }
        public GamePage GamePage { get; }
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

        public MainPageViewModel(SmallGamePage smallGamePage, GamePage gamePage, BoardEditor boardEditor)
        {
            SmallGamePage = smallGamePage;
            GamePage = gamePage;
            BoardEditor = boardEditor;

            var tmpTheme = (string)RoamingSettings["Theme"];
            SelectedTheme = string.IsNullOrEmpty(tmpTheme) ? Theme.Wood : (Theme)Enum.Parse(typeof(Theme), tmpTheme);

            var tmpEnableSoundEffects = (string)RoamingSettings["EnableSoundEffects"];
            EnableSoundEffects = string.IsNullOrEmpty(tmpEnableSoundEffects) || bool.Parse(tmpEnableSoundEffects);
        }

        private void Navigate(Frame frame, Page page) => frame.Content = page;


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

                _gamePageNavigationCommand = new DelegateCommand(param => Navigate((Frame)param, SmallGamePage));
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

                _boardEditorNavigationCommand = new DelegateCommand(param => Navigate((Frame)param, BoardEditor));
                return _boardEditorNavigationCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}