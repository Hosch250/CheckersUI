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

        //private bool ElementCapturesClick(FrameworkElement element, Point mousePosition)
        //{
        //    var transform = element.TransformToVisual(this);
        //    var startPoint = transform.TransformPoint(new Point(0, 0));
        //    var endPoint = new Point(startPoint.X + element.ActualWidth, startPoint.Y + element.ActualHeight);

        //    if (mousePosition.X < startPoint.X || mousePosition.X > endPoint.X ||
        //        mousePosition.Y < startPoint.Y || mousePosition.Y > endPoint.Y)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private void MainPage_PointerPressed(CoreWindow sender, PointerEventArgs args)
        //{
        //    if (SettingsGrid.Visibility == Visibility.Collapsed ||
        //        SettingsToggleButton.IsChecked != true)
        //    {
        //        return;
        //    }

        //    if (ElementCapturesClick(SettingsToggleButton, args.CurrentPoint.Position) ||
        //        ElementCapturesClick(SettingsGrid, args.CurrentPoint.Position))
        //    {
        //        return;
        //    }

        //    SettingsToggleButton.IsChecked = false;
        //}
    }
}
