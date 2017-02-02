using System.Collections.Generic;

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
    }
}