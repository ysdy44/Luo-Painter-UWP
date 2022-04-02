using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class BitmapRegionPage : Page
    {

        BitmapLayer BitmapLayer;
        readonly CanvasTextFormat TextFormat = new CanvasTextFormat
        {
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            FontWeight = FontWeights.Bold
        };

        public BitmapRegionPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructCanvas()
        {
            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.BitmapLayer = new BitmapLayer(sender, 512, 512);
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                //@DPI 
                args.DrawingSession.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                args.DrawingSession.DrawRectangle(0, 0, this.BitmapLayer.Width, this.BitmapLayer.Height, Colors.Red);

                foreach (int i in this.BitmapLayer.GetHitIndexs())
                {
                    int x = this.BitmapLayer.GetX(i);
                    int y = this.BitmapLayer.GetY(i);
                    int left = this.BitmapLayer.GetLeft(x);
                    int top = this.BitmapLayer.GetTop(y);
                    int width = this.BitmapLayer.GetWidth(x);
                    int height = this.BitmapLayer.GetHeight(y);

                    args.DrawingSession.DrawRectangle(left, top, width, height, Colors.Red);
                    args.DrawingSession.DrawText(i.ToString(), left, top, width, height, Colors.Red, this.TextFormat);
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, properties) =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, properties) =>
            {
                Vector2 position = this.CanvasControl.Dpi.ConvertDipsToPixels(point);

                Rect rect = position.GetRect(25);
                this.BitmapLayer.Hit(rect);
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Complete += (point, properties) =>
            {
                IHistory history = this.BitmapLayer.GetBitmapHistory();
                this.CanvasControl.Invalidate(); // Invalidate
            };
        }

    }
}