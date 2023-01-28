using Microsoft.Graphics.Canvas;

namespace Luo_Painter.Brushes
{
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