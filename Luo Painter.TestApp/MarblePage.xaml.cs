using Luo_Painter.Elements;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MarblePage : Page
    {

        readonly Marble MarbleX = new Marble();
        readonly Marble MarbleY = new Marble();

        public MarblePage()
        {
            this.InitializeComponent();
            this.Button.Click += (s, e) => this.Storyboard2.Begin(); // Storyboard

            this.ToggleButton.Unchecked += (s, e) => this.Storyboard1.Begin(); // Storyboard
            this.ToggleButton.Checked += (s, e) => this.Storyboard0.Begin(); // Storyboard

            this.MarbleBorder.ManipulationDelta += (s, e) =>
            {
                int radius = this.ToggleButton.IsChecked == true ? 30 : 100;
                switch (base.FlowDirection)
                {
                    case Windows.UI.Xaml.FlowDirection.LeftToRight:
                        this.TranslateTransform.X = -100 + this.MarbleX.Move(this.TranslateTransform.X + 100, e.Delta.Translation.X, e.IsInertial, radius, base.ActualWidth - radius);
                        break;
                    case Windows.UI.Xaml.FlowDirection.RightToLeft:
                        this.TranslateTransform.X = -100 + this.MarbleX.Move(this.TranslateTransform.X + 100, -e.Delta.Translation.X, e.IsInertial, radius, base.ActualWidth - radius);
                        break;
                    default:
                        break;
                }
                this.TranslateTransform.Y = -100 + this.MarbleY.Move(this.TranslateTransform.Y + 100, e.Delta.Translation.Y, e.IsInertial, radius, base.ActualHeight - radius);
            };
        }
    }
}