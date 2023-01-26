using Luo_Painter.Layers;

namespace Luo_Painter
{
    public sealed partial class BrushPage
    {

        private void PaintBrush_Start()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State != default) return;

            //@Paint
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size, this.InkPresenter.IgnoreSizePressure);
            this.TasksStarted(cap);

            //@Paint
            this.TasksStart();
        }

        private void PaintBrush_Delta()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing, this.InkPresenter.IgnoreSizePressure);
            if (segment.InRadius) return;

            //@Paint
            this.Tasks.Add(segment);

            this.StartingPosition = this.Position;
            this.StartingPressure = this.Pressure;
        }

        private void PaintBrush_Complete()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            //@Paint
            this.TasksStop();
        }

    }
}