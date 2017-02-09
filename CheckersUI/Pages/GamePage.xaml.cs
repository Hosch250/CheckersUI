using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class GamePage
    {
        public GamePage()
        {
            InitializeComponent();
        }

        private GamePageViewModel ViewModel => (GamePageViewModel) DataContext;

        private void RadioButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
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
