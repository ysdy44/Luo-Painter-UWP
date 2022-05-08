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

        public ExpanderPage()
        {
            this.InitializeComponent();
            this.Button.Click += (s, e) => this.Expander.ShowAt(this.Button, ExpanderPlacementMode.Center);
            this.LeftButton.Click += (s, e) => this.Expander.ShowAt(this.LeftButton, ExpanderPlacementMode.Right);
            this.TopButton.Click += (s, e) => this.Expander.ShowAt(this.TopButton, ExpanderPlacementMode.Bottom);
            this.RightButton.Click += (s, e) => this.Expander.ShowAt(this.RightButton, ExpanderPlacementMode.Left);
            this.BottomButton.Click += (s, e) => this.Expander.ShowAt(this.BottomButton, ExpanderPlacementMode.Top);
        }

    }
}