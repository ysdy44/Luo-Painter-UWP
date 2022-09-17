using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class SpotterPatternPage : Page
    {
        int W;
        int H;

        public SpotterPatternPage()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                W = (int)e.NewSize.Width / 50;
                H = (int)e.NewSize.Height / 50;

                CanvasControl.Width = W * 50;
                CanvasControl.Height = H * 50;
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                for (int i = 1; i < W; i++)
                {
                    float x = i * 50;
                    for (int j = 1; j < H; j++)
                    {
                        args.DrawingSession.FillCircle(x, j * 50, x / W / 2, Colors.White);
                    }
                }
            };
        }
    }
}