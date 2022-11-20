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

        private void PaintCapAsync(StrokeCap cap)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);
            }

            this.CanvasControl.Invalidate(); // Invalidate
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
                        StrokeSegment segment = this.Tasks.First();
                        this.Tasks.RemoveAt(0);

                        //@Task
                        lock (this.Locker)
                        {
                            this.BitmapLayer.Hit(segment.Bounds);
                            this.PaintSegment(segment);
                        }

                        await this.CanvasControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            this.CanvasControl.Invalidate();
                        });
                        break;
                    default:
                        //@Paint
                        this.Tasks.State = PaintTaskState.Finished;
                        this.Tasks.Clear();

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
                        }

                        await this.CanvasControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            // History
                            this.BitmapLayer.Flush();
                            this.BitmapLayer.RenderThumbnail();

                            this.CanvasControl.Invalidate(); // Invalidate
                        });
                        return;
                }
            }
        }

        private async void Paint_Start()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.Tasks.State == default) return;

            //@Task
            {
                StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);
                lock (this.Locker)
                {
                    this.BitmapLayer.Hit(cap.Bounds);
                    this.PaintCap(cap);
                }

                this.CanvasControl.Invalidate(); // Invalidate
            }

            //@Paint
            this.Tasks.State = PaintTaskState.Painting;
            await Task.Run(this.PaintAsync);
        }
        private void Paint_Delta()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;

            //@Paint
            this.Tasks.Add(segment);

            this.StartingPosition = this.Position;
            this.StartingPressure = this.Pressure;
        }
        private void Paint_Complete()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            //@Paint
            this.Tasks.State = PaintTaskState.Painted;
        }

    }
}