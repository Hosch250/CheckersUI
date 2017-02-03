using System.Collections.Generic;
using CheckersUI.Command;

namespace CheckersUI.VMs
{
    public class SmallGamePageViewModel : GamePageViewModel
    {
        private readonly MainPageViewModel _vm;

        public SmallGamePageViewModel(MainPageViewModel vm)
        {
            _vm = vm;
        }

        public List<string> Pages { get; } = new List<string> {"Game Page", "Board Editor"};
        public string NavigationElement
        {
            get { return "Game Page"; }
            set
            {
                if (value == "Board Editor")
                {
                    _vm.BoardEditorNavigationCommand.Execute(null);
                }
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
    }
}