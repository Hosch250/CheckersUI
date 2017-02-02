using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class MainPage
    {
        public MainPage(Page initialView)
        {
            InitializeComponent();
            Frame.Content = initialView;
            CoreWindow.GetForCurrentThread().PointerPressed += MainPage_PointerPressed;

            DataContextChanged += MainPage_DataContextChanged;
        }

        private MainPageViewModel ViewModel => (MainPageViewModel) DataContext;

        private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ((MainPageViewModel)args.NewValue).Navigate += MainPage_Navigate;
        }

        private void MainPage_Navigate(object sender, string pageName)
        {
            switch (pageName)
            {
                case "Board Editor":
                    Frame.Content = ViewModel.BoardEditor;
                    break;
                case "Game Page":
                    Frame.Content = ViewModel.GamePage;
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

        private void MainPage_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            if (SettingsGrid.Visibility == Visibility.Collapsed ||
                SettingsToggleButton.IsChecked != true)
            {
                return;
            }

            if (ElementCapturesClick(SettingsToggleButton, args.CurrentPoint.Position) ||
                ElementCapturesClick(SettingsGrid, args.CurrentPoint.Position))
            {
                return;
            }

            SettingsToggleButton.IsChecked = false;
        }
    }
}
