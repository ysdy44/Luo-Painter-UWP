using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;

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
                this.ToastTip.Tip(TipType.NoLayer.GetString(), TipType.NoLayer.GetString(true));
                return;
            }

            this.BitmapLayer = this.LayerSelectedItem as BitmapLayer;
            if (this.BitmapLayer is null)
            {
                this.ToastTip.Tip(TipType.NotBitmapLayer.GetString(), TipType.NotBitmapLayer.GetString(true));
                return;
            }

            this.CanvasAnimatedControl.Paused = true; // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);
            this.PaintStarted(cap);

            //@Paint
            this.TasksStart();
        }

        private void PaintBrush_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
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
            this.TasksStop();
        }

    }
}