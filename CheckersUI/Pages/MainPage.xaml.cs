using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class MainPage
    {
        private readonly GamePage _gamePage;
        private readonly BoardEditor _boardEditor;

        public MainPage(GamePage gamePage, BoardEditor boardEditor)
        {
            InitializeComponent();
            Frame.Content = gamePage;

            _gamePage = gamePage;
            _boardEditor = boardEditor;

            ((INavigatable)_gamePage.DataContext).NavigationRequest += NavigationHandler;
            ((INavigatable)_boardEditor.DataContext).NavigationRequest += NavigationHandler;
        }

        private void NavigationHandler(object sender, string pageName)
        {
            switch (pageName)
            {
                case "Board Editor":
                    Frame.Content = _boardEditor;
                    break;
                case "Game Page":
                    Frame.Content = _gamePage;
                    break;
                default:
                    throw new System.ArgumentException(nameof(pageName));
            }
        }

        private bool ElementCapturesClick(FrameworkElement element, Point mousePosition)
        {
            var transform = element.TransformToVisual(this);
            var startPoint = transform.TransformPoint(new Point(0, 0));
            var endPoint = new Point(startPoint.X + element.ActualWidth, startPoint.Y + element.ActualHeight);

            if (mousePosition.X < startPoint.X || mousePosition.X > endPoint.X ||
                mousePosition.Y < startPoint.Y || mousePosition.Y > endPoint.Y)
            {
                return false;
            }

            return true;
        }

        private void MainPage_PointerPressed(object sender, PointerRoutedEventArgs args)
        {
            if (SettingsGrid.Visibility == Visibility.Collapsed ||
                SettingsToggleButton.IsChecked != true)
            {
                return;
            }

            if (ElementCapturesClick(SettingsToggleButton, args.GetCurrentPoint(this).Position) ||
                ElementCapturesClick(SettingsGrid, args.GetCurrentPoint(this).Position))
            {
                return;
            }

            SettingsToggleButton.IsChecked = false;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var caller = (HyperlinkButton)sender;
            if (caller == GamePageButton)
            {
                NavigationHandler(sender, "Game Page");
            }
            else if (caller == BoardEditorButton)
            {
                NavigationHandler(sender, "Board Editor");
            }
            else if (caller == RulesButton)
            {
                NavigationHandler(sender, "Rules");
            }
        }
    }
}
