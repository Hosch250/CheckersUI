using CheckersUI.VMs;

namespace CheckersUI.CustomControls
{
    public sealed partial class SettingsControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}
