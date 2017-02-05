using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace CheckersUI.Pages
{
    public sealed partial class GamePage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        private void RadioButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
        }
    }
}
