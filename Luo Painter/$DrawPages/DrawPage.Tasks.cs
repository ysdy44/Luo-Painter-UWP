using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {
        private void TasksStop()
        {
            this.Tasks.State = PaintTaskState.Painted;
        }
        private async void TasksStart()
        {
            this.Tasks.State = PaintTaskState.Painting;
            await Task.Run(this.TasksAction);
        }

        private void TasksAction()
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
                        this.PaintDelta(segment);
                        break;
                    default:
                        //@Paint
                        this.Tasks.State = PaintTaskState.Finished;
                        this.Tasks.Clear();
                        this.PaintCompleted();
                        return;
                }
            }
        }

        private void PaintStarted(StrokeCap cap)
        {
            //@Task
            lock (this.Locker)
            {
                //@Debug
                // BitmapLayer is null
                // System.NullReferenceException:“Object reference not set to an instance of an object.”
                if (this.BitmapLayer is null) return;

                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);
            }

            Rect? region = RectExtensions.TryGetRect(cap.StartingPosition, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(cap.StartingSize * this.Transformer.Scale));
            if (region.HasValue)
                this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
        }
        private async void PaintDelta(StrokeSegment segment)
        {
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
        }
        private async void PaintCompleted()
        {
            BitmapLayer bitmapLayer = this.BitmapLayer;

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
        }

    }
}