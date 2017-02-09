using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class SmallGamePage
    {
        public SmallGamePage()
        {
            InitializeComponent();

            DataContextChanged += GamePage_DataContextChanged;
        }

        private void GamePage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ViewModel.MoveUndone += ViewModel_MoveUndone;
        }

        private void ViewModel_MoveUndone(object sender, System.EventArgs e)
        {
            Board.Selection = null;
        }

        private GamePageViewModel ViewModel => (GamePageViewModel) DataContext;

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

        private void CloseAppBar(object sender, RoutedEventArgs e) =>
            BottomAppBar.IsOpen = false;

        private void Main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Board.MaxHeight = ActualHeight - 168;
            GameStatus.Width = Board.ActualWidth;
        }

        private void EightPieceBoard_SelectionChanged(object sender, Facade.Coord e)
        {
            Board.ClearBorders();

            if (e == null ||
                ViewModel.Controller.Board[e] == null ||
                ViewModel.Controller.Board[e].Player != ViewModel.Controller.CurrentPlayer)
            {
                return;
            }

            Board.SetBorder(e);
        }
    }
}
