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

        private void Paint_Start()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

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

            Rect? region = RectExtensions.TryGetRect(this.StartingPoint, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
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
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);
            if (segment.InRadius) return;

            Rect? region = RectExtensions.TryGetRect(this.StartingPoint, this.Point, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
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
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

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
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);
            }

            await CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
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
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(segment.Bounds);
                this.PaintSegment(segment);
            }

            await CanvasVirtualControl.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
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
                int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
                this.BitmapLayer.Flush();
                this.BitmapLayer.RenderThumbnail();

                this.BitmapLayer = null;
                this.CanvasVirtualControl.Invalidate(); // Invalidate

                this.RaiseHistoryCanExecuteChanged();
            }
        }

    }
}