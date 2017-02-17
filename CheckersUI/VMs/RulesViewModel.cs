using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckersUI.VMs
{
    public class RulesViewModel : INotifyPropertyChanged, INavigatable
    {
        public List<string> Pages { get; } = new List<string> { "Game Page", "Board Editor", "Rules" };
        public string NavigationElement
        {
            get { return "Rules"; }
            set
            {
                if (value != "Rules")
                {
                    OnNavigationRequest(value);
                }
            }
        }

        public event EventHandler<string> NavigationRequest;
        protected virtual void OnNavigationRequest(string target) =>
            NavigationRequest?.Invoke(this, target);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}