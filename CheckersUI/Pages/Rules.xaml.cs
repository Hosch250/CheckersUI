using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CheckersUI.Pages
{
    public sealed partial class Rules : IResizable
    {
        public Rules()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BottomAppBar.IsOpen = false;
            ((ComboBox)sender).SelectedIndex = 3;
        }

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
            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;

            MasterGrid.ColumnDefinitions[0].Width = new GridLength(0);
            MasterGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            MasterGrid.Margin = new Thickness(10, 0, 10, 0);

            MasterGrid.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private void LoadDefaultLayout()
        {
            BottomAppBar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;

            MasterGrid.ColumnDefinitions[0].Width = new GridLength(190);
            MasterGrid.ColumnDefinitions[1].Width = new GridLength(640);

            MasterGrid.Margin = new Thickness(0);

            MasterGrid.HorizontalAlignment = HorizontalAlignment.Left;
        }
    }
}
