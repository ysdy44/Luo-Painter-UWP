using Luo_Painter.Elements;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class MarblePage : Page
    {

        bool InertiaStarting = false;
        readonly Marble Marble = new Marble();

        public MarblePage()
        {
            this.InitializeComponent();
            this.Button.Click += (s, e) => this.Storyboard2.Begin(); // Storyboard

            this.ToggleButton.Unchecked += (s, e) => this.Storyboard1.Begin(); // Storyboard
            this.ToggleButton.Checked += (s, e) => this.Storyboard0.Begin(); // Storyboard 

            this.MarbleBorder.ManipulationInertiaStarting += (s, e) => this.InertiaStarting = true;
            this.MarbleBorder.ManipulationDelta += (s, e) =>
            {
                Point p = this.Marble.Move(this.TranslateTransform.X, this.TranslateTransform.Y, e.Delta.Translation, base.ActualWidth - 200, base.ActualHeight - 200, this.InertiaStarting);
                this.TranslateTransform.X = p.X;
                this.TranslateTransform.Y = p.Y;
            };
            this.MarbleBorder.ManipulationCompleted += (s, e) => this.InertiaStarting = false;
        }
    }
}