using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        private async void Paint_Start()
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

            //@Task
            {
                StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);
                lock (this.Locker)
                {
                    this.BitmapLayer.Hit(cap.Bounds);
                    this.PaintCap(cap);
                }

                Rect? region = RectExtensions.TryGetRect(this.StartingPoint, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(cap.Size * this.Transformer.Scale));
                if (region.HasValue)
                {
                    await CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
                    });
                }
            }

            //@Paint
            this.Tasks.State = PaintTaskState.Painting;
            await Task.Run(this.PaintAsync);
        }
        private void Paint_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;

            //@Paint
            this.Tasks.Add(segment);

            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
            this.StartingPressure = this.Pressure;
        }
        private void Paint_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            //@Paint
            this.Tasks.State = PaintTaskState.Painted;
        }

        private async void PaintAsync()
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
                        this.Tasks.Remove(segment);

                        //@Task
                        lock (this.Locker)
                        {
                            this.BitmapLayer.Hit(segment.Bounds);
                            this.PaintSegment(segment);
                        }

                        Rect? region = RectExtensions.TryGetRect(this.ToPoint(segment.StartingPosition), this.ToPoint(segment.Position), this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(segment.Size * this.Transformer.Scale));
                        if (region.HasValue)
                        {
                            await CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                            {
                                this.CanvasVirtualControl.Invalidate(region.Value);
                            });
                        }
                        break;
                    default:
                        //@Paint
                        this.Tasks.State = PaintTaskState.Finished;

                        await CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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
                                int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                                this.BitmapLayer.Flush();
                                this.BitmapLayer.RenderThumbnail();

                                this.BitmapLayer = null;
                                this.CanvasVirtualControl.Invalidate(); // Invalidate

                                this.RaiseHistoryCanExecuteChanged();
                            }
                        });
                        return;
                }
            }
        }

    }
}