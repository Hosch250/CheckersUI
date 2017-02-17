using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CheckersUI.Facade;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class GamePage : IResizable
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
            Board.ClearPrompts();

            if (e == Player.White && ViewModel.WhiteOpponent == Opponent.Human ||
                e == Player.Black && ViewModel.BlackOpponent == Opponent.Human)
            {
                SetMoveHints();
            }
        }

        private void ViewModel_MoveUndone(object sender, System.EventArgs e)
        {
            Board.Selection = null;
        }

        private GamePageViewModel ViewModel => (GamePageViewModel) DataContext;

        private void EightPieceBoard_SelectionChanged(object sender, Coord e)
        {
            Board.ClearBorders();
            Board.ClearPrompts();

            if (ViewModel.Controller.CurrentPlayer == Player.White && ViewModel.WhiteOpponent == Opponent.Human ||
                ViewModel.Controller.CurrentPlayer == Player.Black && ViewModel.BlackOpponent == Opponent.Human)
            {
                SetMoveHints(e);
            }
        }

        private bool AreHintsEnabled()
        {
            var isMoveHintsEnabled = (string)ApplicationData.Current.RoamingSettings.Values["EnableMoveHints"];

            if (string.IsNullOrEmpty(isMoveHintsEnabled)) { return false; }

            return bool.Parse(isMoveHintsEnabled);
        }

        private void SetMoveHints(Coord coord = null)
        {
            var areHintsEnabled = AreHintsEnabled();

            var validMoves = ViewModel.Controller.GetValidMoves();
            var validstartingCoords = validMoves.Select(c => c[0]).Distinct().ToList();
            if (coord == null || !validstartingCoords.Contains(coord))
            {
                if (areHintsEnabled)
                {
                    foreach (var move in validstartingCoords)
                    {
                        Board.SetBorder(move);
                    }
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

                if (areHintsEnabled)
                {
                    Board.SetBorder(ViewModel.Controller.CurrentCoord);
                    SetPrompts(coord, validMoves);
                }

                return;
            }

            if (ViewModel.Controller.Board[coord] != null)
            {
                if (areHintsEnabled)
                {
                    Board.SetBorder(coord);
                    SetPrompts(coord, validMoves);
                }
            }
        }

        private void SetPrompts(Coord coord, List<List<Coord>> moves)
        {
            foreach (var move in moves)
            {
                if (!Equals(move[0], coord)) { continue; }
                Board.SetPrompt(move[1]);
            }
        }

        private void MoveMenu_Tapped(object sender, TappedRoutedEventArgs e) =>
            SmallMoveHistory.Visibility = Visibility.Visible;

        private void MoveHistory_OnMoveSelection(object sender, EventArgs e) =>
            SmallMoveHistory.Visibility = Visibility.Collapsed;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
            ((ComboBox)sender).SelectedIndex = 0;
        }

        private void CloseAppBar(object sender, RoutedEventArgs e) =>
            BottomAppBar.IsOpen = false;
        
        public void LoadLayout(PageLayout layout)
        {
            switch (layout)
            {
                case PageLayout.Default:
                    LoadDefaultLayout();
                    break;
                case PageLayout.Small:
                    LoadSmallLayout();
                    break;
                default:
                    throw new ArgumentException(nameof(layout));
            }
        }

        private void LoadSmallLayout()
        {
            MasterGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            MasterGrid.ColumnDefinitions[1].Width = new GridLength(0);
            MasterGrid.ColumnDefinitions[2].Width = new GridLength(0);

            MasterGrid.RowDefinitions[0].Height = new GridLength(30);
            MasterGrid.RowDefinitions[1].Height = new GridLength(30);
            MasterGrid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            MasterGrid.RowDefinitions[3].Height = new GridLength(60);

            MasterGrid.Margin = new Thickness(10, 0, 10, 0);

            GameStatus.Visibility = Visibility.Collapsed;
            SmallGameStatus.Visibility = Visibility.Visible;
            GameStatus_Variant.Visibility = Visibility.Visible;
            MoveMenu.Visibility = Visibility.Visible;

            Grid.SetRow(Board, 2);
            Grid.SetColumn(Board, 0);

            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
        }

        private void LoadDefaultLayout()
        {
            MasterGrid.ColumnDefinitions[0].Width = new GridLength(190);
            MasterGrid.ColumnDefinitions[1].Width = new GridLength(640);
            MasterGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);

            MasterGrid.RowDefinitions[0].Height = new GridLength(640);
            MasterGrid.RowDefinitions[1].Height = new GridLength(0);
            MasterGrid.RowDefinitions[2].Height = new GridLength(0);
            MasterGrid.RowDefinitions[3].Height = new GridLength(0);

            MasterGrid.Margin = new Thickness(0);

            GameStatus.Visibility = Visibility.Visible;
            SmallGameStatus.Visibility = Visibility.Collapsed;
            GameStatus_Variant.Visibility = Visibility.Collapsed;
            MoveMenu.Visibility = Visibility.Collapsed;

            Grid.SetRow(Board, 0);
            Grid.SetColumn(Board, 1);

            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
        }
    }
}
