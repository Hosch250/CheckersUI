using Windows.UI.Xaml.Controls;
using CheckersUI.Command;
using CheckersUI.Pages;

namespace CheckersUI.VMs
{
    public class MainPageViewModel
    {
        public GamePage GamePage { get; }
        public BoardEditor BoardEditor { get; }

        public MainPageViewModel(GamePage gamePage, BoardEditor boardEditor)
        {
            GamePage = gamePage;
            BoardEditor = boardEditor;
        }

        private void Navigate(Frame frame, Page page) => frame.Content = page;

        private DelegateCommand _gamePageNavigationCommand;
        public DelegateCommand GamePageNavigationCommand
        {
            get
            {
                if (_gamePageNavigationCommand != null)
                {
                    return _gamePageNavigationCommand;
                }

                _gamePageNavigationCommand = new DelegateCommand(param => Navigate((Frame)param, GamePage));
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
    }
}