using Windows.UI.Xaml;

namespace CheckersUI.CustomControls
{
    public sealed partial class AppBarMenu
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(EightPieceBoard), null);

        public AppBarMenu()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
