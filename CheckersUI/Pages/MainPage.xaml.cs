using Windows.UI.Xaml.Controls;

namespace CheckersUI.Pages
{
    public sealed partial class MainPage
    {
        public MainPage(Page initialView)
        {
            InitializeComponent();
            Frame.Content = initialView;
        }
    }
}
