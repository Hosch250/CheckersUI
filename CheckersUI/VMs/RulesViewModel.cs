using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckersUI.VMs
{
    public class RulesViewModel : NavigationViewModel, INotifyPropertyChanged
    {
        public override string NavigationElement
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}