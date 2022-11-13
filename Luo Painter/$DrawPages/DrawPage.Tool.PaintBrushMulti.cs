using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        int PaintBrushMultiMode => this.PaintBrushMultiComboBox.SelectedIndex;

        private async void PaintBrushMulti_Start()
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

            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);
            this.PaintCapAsync(cap);
            switch (this.PaintBrushMultiMode)
            {
                case 0: this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center, Orientation.Horizontal)); break;
                case 1: this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center, Orientation.Vertical)); break;
                case 2: this.PaintCapAsync(new StrokeCap(cap, this.BitmapLayer.Center)); break;
                default: break;
            }

            //@Paint
            this.Tasks.State = PaintTaskState.Painting;
            await Task.Run(this.PaintSegmentAsync);
        }

        private void PaintBrushMulti_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;

            //@Paint
            this.Tasks.Add(segment);
            switch (this.PaintBrushMultiMode)
            {
                case 0: this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center, Orientation.Horizontal)); break;
                case 1: this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center, Orientation.Vertical)); break;
                case 2: this.Tasks.Add(new StrokeSegment(segment, this.BitmapLayer.Center)); break;
                default: break;
            }

            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
            this.StartingPressure = this.Pressure;
        }

        private void PaintBrushMulti_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.CanvasControl.Invalidate(); // Invalidate

            //@Paint
            this.Tasks.State = PaintTaskState.Painted;
        }

    }
}