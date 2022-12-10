using Microsoft.Graphics.Canvas;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.HSVColorPickers
{
    public sealed partial class Strawer : Canvas
    {

        CanvasRenderTarget StrawRender;

        //@Content
        public ICanvasImage StrawImage => this.StrawRender;
        public int StrawCenterX { get; private set; }
        public int StrawCenterY { get; private set; }

        //@Construct
        public Strawer()
        {
            this.InitializeComponent();
        }

        public Color GetStraw() => this.StrawRender.GetPixelColors(this.StrawCenterX, this.StrawCenterY, 1, 1).Single();
        public CanvasDrawingSession CreateDrawingSession() => this.StrawRender.CreateDrawingSession();
        public void CreateResources(ICanvasResourceCreatorWithDpi resourceCreator)
        {
            this.StrawRender = new CanvasRenderTarget(resourceCreator, 112, 90);
            this.StrawCenterX = (int)this.StrawRender.SizeInPixels.Width / 2;
            this.StrawCenterY = (int)this.StrawRender.SizeInPixels.Height / 2;
        }

        public void Show() => this.StrawStoryboard.Begin(); // Storyboard
        public void Hide() => this.HideStoryboard.Begin(); // Storyboard
        public void Move(double x, double y)
        {
            Canvas.SetLeft(this, x - 65);
            Canvas.SetTop(this, y - 130);
        }

    }
}