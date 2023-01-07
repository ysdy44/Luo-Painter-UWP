using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        private void PaintCapAsync(StrokeCap cap) => this.PaintCapAsync(cap, this.ToPoint(cap.StartingPosition));
        private void PaintCapAsync(StrokeCap cap, Vector2 startingPoint)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);
            }

            Rect? region = RectExtensions.TryGetRect(startingPoint, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(cap.Size * this.Transformer.Scale));
            if (region.HasValue)
                this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
        }

        private async void PaintSegmentAsync()
        {
            while (true)
            {
                switch (this.Tasks.GetBehavior())
                {
                    case PaintTaskBehavior.WaitingWork:
                        continue;
                    case PaintTaskBehavior.Working:
                    case PaintTaskBehavior.WorkingBeforeDead:
                        StrokeSegment segment = this.Tasks[0];
                        this.Tasks.RemoveAt(0);

                        //@Task
                        lock (this.Locker)
                        {
                            this.BitmapLayer.Hit(segment.Bounds);
                            this.PaintSegment(segment);
                        }

                        await this.CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            Rect? region = RectExtensions.TryGetRect(this.ToPoint(segment.StartingPosition), this.ToPoint(segment.Position), this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(segment.Size * this.Transformer.Scale));
                            if (region.HasValue)
                            {
                                this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
                            }
                        });
                        break;
                    default:
                        //@Paint
                        this.Tasks.State = PaintTaskState.Finished;
                        this.Tasks.Clear();


                        var bitmapLayer = this.BitmapLayer;

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
                            this.BitmapLayer = null;
                        }

                        await this.CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            // History
                            int removes = this.History.Push(bitmapLayer.GetBitmapHistory());
                            bitmapLayer.Flush();
                            bitmapLayer.RenderThumbnail();

                            this.CanvasVirtualControl.Invalidate(); // Invalidate

                            this.RaiseHistoryCanExecuteChanged();
                        });
                        return;
                }
            }
        }

    }
}