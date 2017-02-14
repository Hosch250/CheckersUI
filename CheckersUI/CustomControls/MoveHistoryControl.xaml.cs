using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace CheckersUI.CustomControls
{
    public sealed partial class MoveHistoryControl
    {
        public MoveHistoryControl()
        {
            InitializeComponent();
        }

        private void RadioButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelected();
        }

        public event EventHandler OnMoveSelection;
        private void MoveSelected() =>
            OnMoveSelection?.Invoke(this, EventArgs.Empty);
    }
}
