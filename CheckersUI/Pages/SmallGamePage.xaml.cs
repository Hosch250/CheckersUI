using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace CheckersUI.Pages
{
    public sealed partial class SmallGamePage
    {
        public SmallGamePage()
        {
            InitializeComponent();
        }

        private void RadioButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) =>
            MoveHistory.Visibility = Visibility.Collapsed;

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e) =>
            MoveHistory.Visibility = Visibility.Visible;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
            ((ComboBox)sender).SelectedIndex = 0;
        }
    }
}
