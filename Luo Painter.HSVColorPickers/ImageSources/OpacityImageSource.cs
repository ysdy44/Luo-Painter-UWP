using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace Luo_Painter.HSVColorPickers
{
    public sealed class OpacityImageSource : GrayAndWhiteImageSource
    {
        public OpacityImageSource(CanvasDevice canvasDevice, int length, int square, float dpi) : base(canvasDevice, length, square, dpi) { }
        protected override void Draw()
        {
            using (CanvasDrawingSession ds = base.CreateDrawingSession(Colors.White))
            {
                for (int i = 0; i < base.Count; i++)
                {
                    ds.FillRectangle(i * this.Square, i % 2 * this.Square, this.Square, this.Square, Colors.LightGray);
                }
            }
        }
    }
}