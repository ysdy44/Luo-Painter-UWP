using Microsoft.Graphics.Canvas;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {
        public CanvasBitmap GrainSource { get; private set; }

        public void ConstructShape(string path, CanvasBitmap source)
        {
            this.Shape = path;
            this.ShapeSource?.Dispose();
            this.ShapeSource = source;
        }
        public void ClearShape()
        {
            this.Shape = null;
            this.ShapeSource?.Dispose();
            this.ShapeSource = null;
        }

        public bool AllowShape // GetType
        {
            get
            {
                if (this.Shape is null) return false;
                if (this.ShapeSource is null) return false;
                return true;
            }
        }
    }
}