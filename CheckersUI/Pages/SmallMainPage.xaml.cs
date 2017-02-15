using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public sealed partial class SmallMainPage
    {
        public SmallMainPage(Page initialView)
        {
            InitializeComponent();
            /*Frame.Content = initialView;
            CoreWindow.GetForCurrentThread().PointerPressed += SmallMainPage_PointerPressed;

            DataContextChanged += SmallMainPage_DataContextChanged;*/
        }

        //private MainPageViewModel ViewModel => (MainPageViewModel)DataContext;

        //private void SmallMainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        //{
        //    ((MainPageViewModel)args.NewValue).Navigate += SmallMainPage_Navigate;
        //}

        //private void SmallMainPage_Navigate(object sender, string pageName)
        //{
        //    switch (pageName)
        //    {
        //        case "Board Editor":
        //            Frame.Content = ViewModel.SmallBoardEditor;
        //            break;
        //        case "Game Page":
        //            Frame.Content = ViewModel.SmallGamePage;
        //            break;
        //        default:
        //            throw new System.ArgumentException(nameof(pageName));
        //    }
        //}

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

        //private void SmallMainPage_PointerPressed(CoreWindow sender, PointerEventArgs args)
        //{
        //    /*if (SettingsGrid.Visibility == Visibility.Collapsed ||
        //        SettingsToggleButton.IsChecked != true)
        //    {
        //        return;
        //    }

        //    if (ElementCapturesClick(SettingsToggleButton, args.CurrentPoint.Position) ||
        //        ElementCapturesClick(SettingsGrid, args.CurrentPoint.Position))
        //    {
        //        return;
        //    }

        //    SettingsToggleButton.IsChecked = false;*/
        //}

        //private void BoardEditorNavigation(object sender, RoutedEventArgs e)
        //{
        //    Frame.Content = ((MainPageViewModel)DataContext).SmallBoardEditor;
        //}
    }
}
