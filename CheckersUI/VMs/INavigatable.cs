using System;

namespace CheckersUI.VMs
{
    public interface INavigatable
    {
        event EventHandler<string> NavigationRequest;
    }
}