using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class SpottedPatternPage : Page
    {
        int Step = 25;
        int W;
        int H;

        public SpottedPatternPage()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                int w = (int)e.NewSize.Width / this.Step;
                int h = (int)e.NewSize.Height / this.Step;

                if (this.W == w && this.H == h) return;
                this.W = w;
                this.H = h;

                w *= this.Step;
                h *= this.Step;
                this.Border.Width = w ;
                this.Border.Height = h;
                this.CanvasControl.Width = w;
                this.CanvasControl.Height = h;
                this.RectangleGeometry.Rect = new Rect(10, 10, w - 20, h - 20);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                for (int i = 1; i < this.W; i++)
                {
                    float x = this.Step * i;
                    for (int j = 1; j < this.H; j++)
                    {
                        args.DrawingSession.FillCircle(x, j * this.Step, x / this.W / 3, Colors.White);
                    }
                }
            };
        }
    }
}