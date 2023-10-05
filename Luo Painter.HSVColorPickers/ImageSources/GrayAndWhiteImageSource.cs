using Microsoft.Graphics.Canvas;

namespace Luo_Painter.HSVColorPickers
{
    public abstract class GrayAndWhiteImageSource : CanvasImageSourceBase
    {
        protected readonly int Count;
        protected readonly int Length;
        protected readonly int Square;

        internal GrayAndWhiteImageSource(CanvasDevice canvasDevice, int length, int square, float dpi) : base(canvasDevice, length, square * 2, dpi)
        {
            this.Count = length / square;
            this.Length = length;
            this.Square = square;
            this.Draw();
        }
    }
}