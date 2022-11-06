using Microsoft.Graphics.Canvas;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes
    {

        public void ConstructShape(string path, CanvasBitmap source)
        {
            this.AllowShape = true;
            this.Shape = path;
            this.ShapeSource?.Dispose();
            this.ShapeSource = source;
        }
        public void ClearShape()
        {
            this.AllowShape = false;
            this.Shape = null;
            this.ShapeSource?.Dispose();
            this.ShapeSource = null;
        }

        public void TurnOffShape() => this.AllowShape = false;
        public bool TryTurnOnShape()
        {
            if (this.AllowShape) return false;
            if (this.Shape is null) return false;
            if (this.ShapeSource is null) return false;
            this.AllowShape = true;
            return true;
        }


        public void ConstructGrain(string path, CanvasBitmap source)
        {
            this.AllowGrain = true;
            this.Grain = path;
            this.GrainSource?.Dispose();
            this.GrainSource = source;
        }
        public void ClearGrain()
        {
            this.AllowGrain = false;
            this.Grain = null;
            this.GrainSource?.Dispose();
            this.GrainSource = null;
        }
        
        public void TurnOffGrain() => this.AllowGrain = false;
        public bool TryTurnOnGrain()
        {
            if (this.AllowGrain) return false;
            if (this.Grain is null) return false;
            if (this.GrainSource is null) return false;
            this.AllowGrain = true;
            return true;
        }

    }
}