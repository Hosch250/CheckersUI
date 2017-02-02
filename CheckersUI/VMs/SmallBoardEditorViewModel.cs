using System.Collections.Generic;
using CheckersUI.Command;
using CheckersUI.Facade;

namespace CheckersUI.VMs
{
    public class SmallBoardEditorViewModel : BoardEditorViewModel
    {
        private readonly MainPageViewModel _vm;

        public SmallBoardEditorViewModel(Board board, MainPageViewModel vm) : base (board)
        {
            _vm = vm;
            DisplayAppBarPrompt = true;
            IsAppBarVisible = true;
        }

        private bool _displayAppBarPrompt;
        public bool DisplayAppBarPrompt
        {
            get
            {
                return _displayAppBarPrompt;
            }
            set
            {
                if (value != _displayAppBarPrompt)
                {
                    _displayAppBarPrompt = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAppBarVisible;
        public bool IsAppBarVisible
        {
            get
            {
                return _isAppBarVisible;
            }
            set
            {
                if (value != _isAppBarVisible)
                {
                    _isAppBarVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isVariantOptionsVisible;
        public bool IsVariantOptionsVisible
        {
            get
            {
                return _isVariantOptionsVisible;
            }
            set
            {
                if (value != _isVariantOptionsVisible)
                {
                    _isVariantOptionsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPlayerOptionsVisible;
        public bool IsPlayerOptionsVisible
        {
            get
            {
                return _isPlayerOptionsVisible;
            }
            set
            {
                if (value != _isPlayerOptionsVisible)
                {
                    _isPlayerOptionsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isBuiltinBoardPositionOptionsVisible;
        public bool IsBuiltinBoardPositionOptionsVisible
        {
            get
            {
                return _isBuiltinBoardPositionOptionsVisible;
            }
            set
            {
                if (value != _isBuiltinBoardPositionOptionsVisible)
                {
                    _isBuiltinBoardPositionOptionsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> Pages { get; } = new List<string> {"Game Page", "Board Editor"};
        public string NavigationElement
        {
            get { return "Board Editor"; }
            set
            {
                if (value == "Game Page")
                {
                    _vm.GamePageNavigationCommand.Execute(null);
                }
            }
        }

        private DelegateCommand _hideAppBarCommand;
        public DelegateCommand HideAppBarCommand
        {
            get
            {
                if (_hideAppBarCommand != null)
                {
                    return _hideAppBarCommand;
                }

                _hideAppBarCommand = new DelegateCommand(param =>
                {
                    IsVariantOptionsVisible = false;
                    IsPlayerOptionsVisible = false;
                    IsBuiltinBoardPositionOptionsVisible = false;
                    IsAppBarVisible = false;
                    DisplayAppBarPrompt = true;
                });
                return _hideAppBarCommand;
            }
        }

        private DelegateCommand _displayAppBarCommand;
        public DelegateCommand DisplayAppBarCommand
        {
            get
            {
                if (_displayAppBarCommand != null)
                {
                    return _displayAppBarCommand;
                }

                _displayAppBarCommand = new DelegateCommand(param =>
                {
                    DisplayAppBarPrompt = false;
                    IsVariantOptionsVisible = false;
                    IsPlayerOptionsVisible = false;
                    IsBuiltinBoardPositionOptionsVisible = false;
                    IsAppBarVisible = true;
                });
                return _displayAppBarCommand;
            }
        }

        private DelegateCommand _displayVariantOptionsCommand;
        public DelegateCommand DisplayVariantOptionsCommand
        {
            get
            {
                if (_displayVariantOptionsCommand != null)
                {
                    return _displayVariantOptionsCommand;
                }

                _displayVariantOptionsCommand = new DelegateCommand(param =>
                {
                    IsVariantOptionsVisible = true;
                    IsAppBarVisible = false;
                });
                return _displayVariantOptionsCommand;
            }
        }

        private DelegateCommand _displayPlayerOptionsCommand;
        public DelegateCommand DisplayPlayerOptionsCommand
        {
            get
            {
                if (_displayPlayerOptionsCommand != null)
                {
                    return _displayPlayerOptionsCommand;
                }

                _displayPlayerOptionsCommand = new DelegateCommand(param =>
                {
                    IsPlayerOptionsVisible = true;
                    IsAppBarVisible = false;
                });
                return _displayPlayerOptionsCommand;
            }
        }

        private DelegateCommand _displayBuiltinBoardPositionOptionsCommand;
        public DelegateCommand DisplayBuiltinBoardPositionOptionsCommand
        {
            get
            {
                if (_displayBuiltinBoardPositionOptionsCommand != null)
                {
                    return _displayBuiltinBoardPositionOptionsCommand;
                }

                _displayBuiltinBoardPositionOptionsCommand = new DelegateCommand(param =>
                {
                    IsBuiltinBoardPositionOptionsVisible = true;
                    IsAppBarVisible = false;
                });
                return _displayBuiltinBoardPositionOptionsCommand;
            }
        }
    }
}