using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
    {

        private void Paint_Start()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            Rect? region = RectExtensions.TryGetRect(this.StartingPosition, this.CanvasControl.Size, this.CanvasControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size));
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);

            if (this.InkType.HasFlag(InkType.General))
            {
                if (region.HasValue)
                    this.Tasks.Add(Task.Run(() => this.PaintCapAsync(cap, region.Value)));
                else
                    this.Tasks.Add(Task.Run(() => this.PaintCapAsync(cap)));
            }
            else
            {
                if (region.HasValue)
                    this.PaintCapAsync(cap, region.Value);
                else
                    this.PaintCapAsync(cap);
            }
        }
        private void Paint_Delta()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;

            Rect? region = RectExtensions.TryGetRect(this.StartingPosition, this.Position, this.CanvasControl.Size, this.CanvasControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size));
            this.StartingPosition = this.Position;
            this.StartingPressure = this.Pressure;


            if (this.InkType.HasFlag(InkType.General))
            {
                if (region.HasValue)
                    this.Tasks.Add(Task.Run(() => this.PaintSegmentAsync(segment, region.Value)));
                else
                    this.Tasks.Add(Task.Run(() => this.PaintSegmentAsync(segment)));
            }
            else
            {
                if (region.HasValue)
                    this.PaintSegmentAsync(segment, region.Value);
                else
                    this.PaintSegmentAsync(segment);
            }
        }
        private async void Paint_Complete()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            if (this.Tasks.Count is 0 is false)
            {
                //@Task
                await Task.WhenAll(this.Tasks.ToArray());
                this.Tasks.Clear();
            }

            this.PaintHistoryAsync();
        }


        private void PaintCapAsync(StrokeCap cap)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);
            }
        }
        private async void PaintCapAsync(StrokeCap cap, Rect region)
        {
            this.PaintCapAsync(cap);
            await CanvasControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            });
        }

        private void PaintSegmentAsync(StrokeSegment segment)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(segment.Bounds);
                this.PaintSegment(segment);
            }
        }
        private async void PaintSegmentAsync(StrokeSegment segment, Rect region)
        {
            this.PaintSegmentAsync(segment);
            await CanvasControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                this.CanvasControl.Invalidate(); // Invalidate
            });
        }

        private void PaintHistoryAsync()
        {
            //@Task
            lock (this.Locker)
            {
                if (this.InkType is InkType.Liquefy is false)
                {
                    using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.Preview(ds, this.InkType, this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]);
                    }
                }
                this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

                // History
                this.BitmapLayer.Flush();

                this.CanvasControl.Invalidate(); // Invalidate
            }
        }

    }
}