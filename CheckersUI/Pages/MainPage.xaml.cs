using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CheckersUI.VMs;

namespace CheckersUI.Pages
{
    public enum PageLayout { Default, Small }

    public sealed partial class MainPage
    {
        private readonly GamePage _gamePage;
        private readonly BoardEditor _boardEditor;
        private readonly Rules _rules;

        public MainPage(GamePage gamePage, BoardEditor boardEditor, Rules rules)
        {
            InitializeComponent();
            Frame.Content = gamePage;

            _gamePage = gamePage;
            _boardEditor = boardEditor;
            _rules = rules;

            ((NavigationViewModel)_gamePage.DataContext).NavigationRequest += NavigationHandler;
            ((NavigationViewModel)_boardEditor.DataContext).NavigationRequest += NavigationHandler;
            ((NavigationViewModel)_rules.DataContext).NavigationRequest += NavigationHandler;
        }

        private void NavigationHandler(object sender, string pageName)
        {
            switch (pageName)
            {
                case "Game Page":
                    Frame.Content = _gamePage;
                    break;
                case "Board Editor":
                    Frame.Content = _boardEditor;
                    break;
                case "Rules":
                    Frame.Content = _rules;
                    break;
                default:
                    throw new ArgumentException(nameof(pageName));
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

        private PageLayout _currentState = PageLayout.Default;
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == 0) { return; }

            if (e.NewSize.Width <= 1180 && _currentState != PageLayout.Small)
            {
                _currentState = PageLayout.Small;
                LoadLayout();
            }
            if (e.NewSize.Width > 1180 && _currentState != PageLayout.Default)
            {
                _currentState = PageLayout.Default;
                LoadLayout();
            }
        }

        private void LoadLayout()
        {
            switch (_currentState)
            {
                case PageLayout.Default:
                    LoadDefaultLayout();
                    break;
                case PageLayout.Small:
                    LoadSmallLayout();
                    break;
                default:
                    throw new ArgumentException(nameof(_currentState));
            }

            _gamePage.LoadLayout(_currentState);
            _boardEditor.LoadLayout(_currentState);
            _rules.LoadLayout(_currentState);
        }

        private void LoadSmallLayout()
        {
            MasterGrid.RowDefinitions[0].Height = new GridLength(50);
            MasterGrid.ColumnDefinitions[0].Width = new GridLength(0);

            NavigationControls.Visibility = Visibility.Collapsed;

            Frame.Padding = new Thickness(0);
        }

        private void LoadDefaultLayout()
        {
            MasterGrid.RowDefinitions[0].Height = new GridLength(30);
            MasterGrid.ColumnDefinitions[0].Width = new GridLength(160);

            NavigationControls.Visibility = Visibility.Visible;
        }
    }
}
