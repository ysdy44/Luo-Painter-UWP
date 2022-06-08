using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Brushes
{
    public sealed class InkRender
    {

        public ICanvasImage Source => this.Bitmap;
        readonly CanvasRenderTarget Bitmap;

        readonly int Width;
        readonly int Height;
        readonly Vector2 Open;
        readonly Vector2 End;

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        public InkRender(ICanvasResourceCreatorWithDpi resourceCreator, int width, int height)
        {
            this.Bitmap = new CanvasRenderTarget(resourceCreator, width, height);
            this.Width = width;
            this.Height = height;
            this.Open = new Vector2(0, height / 2);
            this.End = new Vector2(width, height / 2);
        }

        public void DrawLine(float strokeWidth, Color color)
        {
            Vector2 position = this.Open;
            float x = 0;

            using (CanvasDrawingSession ds = this.Bitmap.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                do
                {
                    // 0 ~ Π
                    float radian = x / this.Width * FanKit.Math.Pi;

                    // Sin 0 ~ 2Π ~
                    float offsetY = 20 * (float)System.Math.Sin(radian + radian);
                    Vector2 targetPosition = new Vector2(x, this.Height / 2 + offsetY);

                    x += strokeWidth * 10;


                    ds.DrawLine(position, targetPosition, color, strokeWidth, this.CanvasStrokeStyle);
                    position = targetPosition;
                } while (x < this.Width);

                ds.DrawLine(position, this.End, color, strokeWidth, this.CanvasStrokeStyle);
            }
        }

        public void IsometricFillCircle(float size, float spacing, Color color)
        {
            Vector2 position;
            float pressure;
            float x = 0;

            using (CanvasDrawingSession ds = this.Bitmap.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                ds.FillCircle(this.Open, 0.001f, color);
                ds.FillCircle(this.End, 0.001f, color);

                do
                {
                    // 0 ~ Π
                    float radian = x / this.Width * FanKit.Math.Pi;

                    // Sin 0 ~ Π ︵
                    pressure = (float)System.Math.Sin(radian);
                    // Sin 0 ~ 2Π ~
                    float offsetY = 20 * (float)System.Math.Sin(radian + radian);
                    position = new Vector2(x, this.Height / 2 + offsetY);


                    float sizePressure = size * pressure + 1;
                    x += sizePressure * spacing;

                    ds.FillCircle(position, sizePressure, color);
                } while (x < this.Width);
            }
        }

    }
}