using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CheckersUI.Facade;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class GamePage
    {
        public GamePage()
        {
            InitializeComponent();

            DataContextChanged += GamePage_DataContextChanged;
        }

        private void GamePage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ViewModel.MoveUndone += ViewModel_MoveUndone;
            ViewModel.PlayerTurn += ViewModel_PlayerTurn;
        }

        private void ViewModel_PlayerTurn(object sender, Player e)
        {
            Board.ClearBorders();

            if (e == Player.White && ViewModel.WhiteOpponent == Opponent.Human ||
                e == Player.Black && ViewModel.BlackOpponent == Opponent.Human)
            {
                SetBorders();
            }
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

        private void EightPieceBoard_SelectionChanged(object sender, Coord e)
        {
            Board.ClearBorders();

            if (ViewModel.Controller.CurrentPlayer == Player.White && ViewModel.WhiteOpponent == Opponent.Human ||
                ViewModel.Controller.CurrentPlayer == Player.Black && ViewModel.BlackOpponent == Opponent.Human)
            {
                SetBorders(e);
            }
        }

        private bool AreHintsEnabled()
        {
            var isMoveHintsEnabled = (string)ApplicationData.Current.RoamingSettings.Values["EnableMoveHints"];

            if (string.IsNullOrEmpty(isMoveHintsEnabled)) { return false; }

            return bool.Parse(isMoveHintsEnabled);
        }

        private void SetBorders(Coord coord = null)
        {
            if (!AreHintsEnabled()) { return; }

            var validstartingCoords = ViewModel.Controller.GetValidMoves().Select(c => c[0]).Distinct().ToList();
            if (coord == null ||
                !validstartingCoords.Contains(coord))
            {
                foreach (var move in validstartingCoords)
                {
                    Board.SetBorder(move);
                }

                if (validstartingCoords.Count == 1)
                {
                    Board.Selection = validstartingCoords[0];
                }

                return;
            }

            if (ViewModel.Controller.CurrentCoord != null)
            {
                Board.Selection = ViewModel.Controller.CurrentCoord;
                Board.SetBorder(ViewModel.Controller.CurrentCoord);
                return;
            }

            if (ViewModel.Controller.Board[coord] != null)
            {
                Board.SetBorder(coord);
            }
        }
    }
}
