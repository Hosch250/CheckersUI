using System;
using System.Collections.Generic;

namespace CheckersUI.VMs
{
    public abstract class NavigationViewModel
    {
        public List<string> Pages { get; } = new List<string> { "Game Page", "Board Editor", "Rules" };
        public abstract string NavigationElement { get; set; }

        public event EventHandler<string> NavigationRequest;
        protected virtual void OnNavigationRequest(string target) =>
            NavigationRequest?.Invoke(this, target);
    }
}