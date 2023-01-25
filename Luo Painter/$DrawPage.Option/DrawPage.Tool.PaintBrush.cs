using Luo_Painter.Blends;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private void PaintBrush_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            if (this.LayerSelectedItem is null)
            {
                this.Tip(TipType.NoLayer);
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.Tip(TipType.NotBitmapLayer);
                return;
            }

            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size, this.InkPresenter.IgnoreSizePressure);
            this.PaintStarted(cap);

            //@Paint
            this.PaintStart();
        }

        private void PaintBrush_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing, this.InkPresenter.IgnoreSizePressure);
            if (segment.InRadius) return;

            //@Paint
            this.Tasks.Add(segment);

            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
            this.StartingPressure = this.Pressure;
        }

        private void PaintBrush_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasAnimatedControl.Paused = this.OptionType.HasPreview(); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            this.PaintStop();
        }

    }
}