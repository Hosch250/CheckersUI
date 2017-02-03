using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public MainPageViewModel(SmallGamePage smallGamePage, GamePage gamePage, SmallBoardEditor smallBoardEditor, BoardEditor boardEditor)
        {
            SmallGamePage = smallGamePage;
            GamePage = gamePage;
            SmallBoardEditor = smallBoardEditor;
            BoardEditor = boardEditor;
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