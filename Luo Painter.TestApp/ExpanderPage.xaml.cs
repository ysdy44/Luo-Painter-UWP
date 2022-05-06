using Luo_Painter.Elements;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.TestApp
{
    public sealed partial class ExpanderPage : Page
    {

        readonly Popup Popup;
        readonly Expander Expander = new Expander
        {
            Title = "Title",
            RequestedTheme = ElementTheme.Dark,
            Background = new SolidColorBrush(Colors.DodgerBlue),
            Content = new ListView
            {
                Width = 300,
                ItemsSource = new int[]
                {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7
                }
            }
        };


        public ExpanderPage()
        {
            this.InitializeComponent();
            this.Popup = new Popup
            {
                Child = new Canvas
                {
                    Children =
                    {
                        this.Expander
                    }
                }
            };

            this.ToggleButton.Unchecked += (s, e) => this.Expander.Hide();
            this.ToggleButton.Checked += (s, e) => this.Expander.ShowAt(this.ToggleButton, ExpanderPlacementMode.Center);

            this.LeftToggleButton.Unchecked += (s, e) => this.Expander.Hide();
            this.LeftToggleButton.Checked += (s, e) => this.Expander.ShowAt(this.LeftToggleButton, ExpanderPlacementMode.Right);

            this.TopToggleButton.Unchecked += (s, e) => this.Expander.Hide();
            this.TopToggleButton.Checked += (s, e) => this.Expander.ShowAt(this.TopToggleButton, ExpanderPlacementMode.Bottom);

            this.RightToggleButton.Unchecked += (s, e) => this.Expander.Hide();
            this.RightToggleButton.Checked += (s, e) => this.Expander.ShowAt(this.RightToggleButton, ExpanderPlacementMode.Left);

            this.BottomToggleButton.Unchecked += (s, e) => this.Expander.Hide();
            this.BottomToggleButton.Checked += (s, e) => this.Expander.ShowAt(this.BottomToggleButton, ExpanderPlacementMode.Top);
        }

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.Popup.IsOpen = false;
        }
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Popup.IsOpen = true;
        }

    }
}