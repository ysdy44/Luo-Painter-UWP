using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class RegionsInvalidatedPage : Page
    {

        Vector2 Position;

        public RegionsInvalidatedPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();
        }

        private void ConstructCanvas()
        {
            this.CanvasVirtualControl.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;
            };
            this.CanvasVirtualControl.RegionsInvalidated += (sender, args) =>
            {
                foreach (Rect region in args.InvalidatedRegions)
                {
                    using (CanvasDrawingSession ds = sender.CreateDrawingSession(region))
                    {
                        ds.DrawRectangle(region, Colors.Red);
                    }
                }
            };
        }

        private void ConstructOperator()
        {
            // Single
            this.Operator.Single_Start += (point, device, properties) =>
            {
                this.Position = point;
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
            this.Operator.Single_Delta += (point, device, properties) =>
            {
                Rect? region = RectExtensions.TryGetRect(point, this.Position, this.CanvasVirtualControl.Size, 10);
                this.Position = point;

                if (region.HasValue)
                    this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
            };
            this.Operator.Single_Complete += (point, device, properties) =>
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

    }
}