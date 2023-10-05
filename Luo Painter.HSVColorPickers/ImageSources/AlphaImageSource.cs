using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace Luo_Painter.HSVColorPickers
{
    public sealed class AlphaImageSource : GrayAndWhiteImageSource
    {
        Color Color0 = Colors.DarkGray;
        Color Color1 = Colors.DimGray;
        public AlphaImageSource(CanvasDevice canvasDevice, int length, int square, float dpi) : base(canvasDevice, length, square, dpi) { }
        protected override void Draw()
        {
            using (CanvasDrawingSession ds = base.CreateDrawingSession(Colors.DodgerBlue))
            {
                for (int i = 0; i < base.Count; i++)
                {
                    this.Color0.A = this.Color1.A = (byte)(255f - 255f * i / base.Count);

                    if (i % 2 is 0)
                    {
                        ds.FillRectangle(i * this.Square, 0, this.Square, this.Square, this.Color0);
                        ds.FillRectangle(i * this.Square, this.Square, this.Square, this.Square, this.Color1);
                    }
                    else
                    {
                        ds.FillRectangle(i * this.Square, 0, this.Square, this.Square, this.Color1);
                        ds.FillRectangle(i * this.Square, this.Square, this.Square, this.Square, this.Color0);
                    }
                }
            }
        }
    }
}