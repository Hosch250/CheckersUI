using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;

namespace CheckersUI.VMs
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
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
                        {"EnableSoundEffects", bool.TrueString},
                        {"EnableMoveHints", bool.FalseString},
                    };
                }
            }
        }

        public SettingsViewModel()
        {
            var tmpTheme = (string)RoamingSettings["Theme"];
            SelectedTheme = string.IsNullOrEmpty(tmpTheme) ? Theme.Wood : (Theme)Enum.Parse(typeof(Theme), tmpTheme);

            var tmpEnableSoundEffects = (string)RoamingSettings["EnableSoundEffects"];
            EnableSoundEffects = string.IsNullOrEmpty(tmpEnableSoundEffects) || bool.Parse(tmpEnableSoundEffects);

            var tmpEnableMoveHints= (string)RoamingSettings["EnableMoveHints"];
            EnableMoveHints = !string.IsNullOrEmpty(tmpEnableMoveHints) && bool.Parse(tmpEnableMoveHints);
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

        private bool _enableMoveHints;
        public bool EnableMoveHints
        {
            get { return _enableMoveHints; }
            set
            {
                if (_enableMoveHints != value)
                {
                    _enableMoveHints = value;
                    OnPropertyChanged();
                }

                AssignRoamingSetting("EnableMoveHints", value.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}