using Microsoft.Graphics.Canvas;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public void ConstructShape(string texture, CanvasBitmap source)
        {
            this.AllowShape = true;
            this.ShapeTexture = texture;
            this.ShapeSource?.Dispose();
            this.ShapeSource = source;
        }
        public void ClearShape()
        {
            this.AllowShape = false;
            this.ShapeTexture = null;
            this.ShapeSource?.Dispose();
            this.ShapeSource = null;
        }

        public void TurnOffShape() => this.AllowShape = false;
        public bool TryTurnOnShape()
        {
            if (this.AllowShape) return false;
            if (this.ShapeTexture is null) return false;
            if (this.ShapeSource is null) return false;
            this.AllowShape = true;
            return true;
        }


        public void ConstructGrain(string texture, CanvasBitmap source)
        {
            this.AllowGrain = true;
            this.GrainTexture = texture;
            this.GrainSource?.Dispose();
            this.GrainSource = source;
        }
        public void ClearGrain()
        {
            this.AllowGrain = false;
            this.GrainTexture = null;
            this.GrainSource?.Dispose();
            this.GrainSource = null;
        }
        
        public void TurnOffGrain() => this.AllowGrain = false;
        public bool TryTurnOnGrain()
        {
            if (this.AllowGrain) return false;
            if (this.GrainTexture is null) return false;
            if (this.GrainSource is null) return false;
            this.AllowGrain = true;
            return true;
        }

    }
}