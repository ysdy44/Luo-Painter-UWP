using Microsoft.Graphics.Canvas;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes
    {
        public CanvasBitmap ShapeSource { get; private set; }
        public bool AllowShape // GetType
        {
            get
            {
                if (base.Shape is null) return false;
                if (this.ShapeSource is null) return false;
                return true;
            }
        }

        public CanvasBitmap GrainSource { get; private set; }
        public bool AllowGrain
        {
            get
            {
                if (base.Grain is null) return false;
                if (this.GrainSource is null) return false;
                return true;
            }
        }

        public void ConstructShape(string path, CanvasBitmap source)
        {
            base.Shape = path;
            this.ShapeSource?.Dispose();
            this.ShapeSource = source;
        }
        public void ClearShape()
        {
            base.Shape = null;
            this.ShapeSource?.Dispose();
            this.ShapeSource = null;
        }

        public void ConstructGrain(string path, CanvasBitmap source)
        {
            base.Grain = path;
            this.GrainSource?.Dispose();
            this.GrainSource = source;
        }
        public void ClearGrain()
        {
            base.Grain = null;
            this.GrainSource?.Dispose();
            this.GrainSource = null;
        }
    }
}