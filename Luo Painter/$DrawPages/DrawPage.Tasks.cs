using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Models;
using Luo_Painter.Strings;
using Microsoft.Graphics.Canvas;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {
        int Timeout;

        private void TasksStop()
        {
            this.Tasks.State = TaskState.Painted;
        }
        private async void TasksStart()
        {
            this.Tasks.State = TaskState.Painting;
            await Task.Run(this.TasksAction);
        }

        private void TasksAction()
        {
            while (true)
            {
                switch (this.Tasks.GetBehavior())
                {
                    case TaskBehavior.WaitingWork:

                        //@Debug
                        // Task waiting freezes the UI thread,
                        // So the Task sleeps for 10 milliseconds.
                        Thread.Sleep(10);

                        this.Timeout++;
                        if (this.Timeout < 512) continue;
                        this.Timeout = 0;

                        //@Paint
                        this.Tasks.State = TaskState.Finished;
                        this.Tasks.Clear();
                        this.PaintCompleted();
                        return;
                    case TaskBehavior.Working:
                    case TaskBehavior.WorkingBeforeDead:
                        this.Timeout = 0;

                        StrokeSegment segment = this.Tasks[0];
                        this.Tasks.RemoveAt(0);
                        this.PaintDelta(segment);
                        break;
                    default:
                        this.Timeout = 0;

                        //@Paint
                        this.Tasks.State = TaskState.Finished;
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
            this.BitmapLayer = null;

            if (bitmapLayer is null is false)
            {
                //@Task
                lock (this.Locker)
                {
                    if (this.InkType is InkType.Liquefy is false)
                    {
                        using (CanvasDrawingSession ds = bitmapLayer.CreateDrawingSession())
                        {
                            ds.Clear(Colors.Transparent);
                            this.InkPresenter.Preview(ds, this.InkType, bitmapLayer[BitmapType.Origin], bitmapLayer[BitmapType.Temp]);
                        }
                    }
                    bitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);
                }
            }

            await this.CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                // History
                IHistory history = bitmapLayer.GetBitmapHistory();
                history.Title = this.OptionType.GetString();
                int removes = this.History.Push(history);

                bitmapLayer.Flush();
                bitmapLayer.RenderThumbnail();

                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.RaiseHistoryCanExecuteChanged();
            });
        }

    }
}