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

    public sealed partial class InkPresenter
    {
        public CanvasBitmap ShapeSource { get; private set; }

        public void ConstructGrain(string path, CanvasBitmap source)
        {
            this.Grain = path;
            this.GrainSource?.Dispose();
            this.GrainSource = source;
        }
        public void ClearGrain()
        {
            this.Grain = null;
            this.GrainSource?.Dispose();
            this.GrainSource = null;
        }

        public bool AllowGrain
        {
            get
            {
                if (this.Grain is null) return false;
                if (this.GrainSource is null) return false;
                return true;
            }
        }
    }
}