using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public void Marquee(Vector2 position, Vector2 targetPosition, float strokeWidth, bool isErasing)
        {
            if (isErasing)
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    ds.Blend = CanvasBlend.Copy;

                    ds.DrawLine(position, targetPosition, Colors.Transparent, strokeWidth, BitmapLayer.CanvasStrokeStyle);
                }
            }
            else
            {
                using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
                {
                    ds.DrawLine(position, targetPosition, Colors.DodgerBlue, strokeWidth, BitmapLayer.CanvasStrokeStyle);
                }
            }
        }

    }
}