using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
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

            if (this.InkType.HasFlag(InkType.Mix)) this.CacheMix(this.StartingPosition);
            if (this.InkType.HasFlag(InkType.Shape) && this.InkPresenter.Rotate) return; // Shape without NaN
            if (this.InkType.HasFlag(InkType.Liquefy)) return; // Liquefy without NaN

            Stroke stroke = new Stroke(this.StartingPoint, this.Point, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);

            //@Task
            if (true)
                this.PaintCapAsync(stroke, cap);
            else
                Task.Run(() => this.PaintCapAsync(stroke, cap));
        }
        private void Paint_Delta()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

            if (segment.InRadius) return;
            if (segment.IsNaN)
            {
                if (this.InkType.HasFlag(InkType.Shape) && this.InkPresenter.Rotate) return; // Shape without NaN
                if (this.InkType.HasFlag(InkType.Liquefy)) return; // Liquefy without NaN
            }
            if (this.InkType.HasFlag(InkType.Mix)) this.Mix(this.Position, this.InkPresenter.Opacity);

            Stroke stroke = new Stroke(this.StartingPoint, this.Point, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));

            //@Task
            if (true)
                this.PaintSegmentAsync(stroke, segment);
            else
                Task.Run(() => this.PaintSegmentAsync(stroke, segment));

            this.StartingPosition = this.Position;
            this.StartingPoint = this.Point;
            this.StartingPressure = this.Pressure;
        }
        private void Paint_Complete()
        {
            if (this.CanvasVirtualControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            //@Task
            if (true)
                this.PaintHistoryAsync();
            else
                Task.Run(this.PaintHistoryAsync);
        }
        
        private void PaintCapAsync(Stroke stroke, StrokeCap cap)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(cap.Bounds);
                this.PaintCap(cap);

                if (stroke.HasIntersect)
                {
                    this.CanvasVirtualControl.Invalidate(stroke.Intersect); // Invalidate
                }
            }
        }
        private void PaintSegmentAsync(Stroke stroke, StrokeSegment segment)
        {
            //@Task
            lock (this.Locker)
            {
                this.BitmapLayer.Hit(segment.Bounds);
                this.PaintSegment(segment);

                if (stroke.HasIntersect)
                {
                    this.CanvasVirtualControl.Invalidate(stroke.Intersect); // Invalidate
                }
            }
        }
        private void PaintHistoryAsync()
        {
            //@Task
            lock (this.Locker)
            {
                this.Paint();
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

        private bool CacheMix(Vector2 position)
        {
            this.MixX = -1;
            this.MixY = -1;

            // 1. Get Position and Target
            int px = (int)position.X;
            int py = (int)position.Y;
            if (this.BitmapLayer.Contains(px, py) is false) return false;

            this.MixX = px;
            this.MixY = py;

            Color target = this.BitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

            // 2. Cache TargetColor with Color
            return this.InkMixer.Cache(target);
        }
        private bool CacheMixAll(Vector2 position)
        {
            this.MixX = -1;
            this.MixY = -1;

            foreach (ILayer item in this.ObservableCollection)
            {
                if (item.Type is LayerType.Bitmap is false) continue;

                if (item is BitmapLayer bitmapLayer)
                {
                    // 1. Get Position and Target
                    int px = (int)position.X;
                    int py = (int)position.Y;
                    if (bitmapLayer.Contains(px, py) is false) continue;

                    this.MixX = px;
                    this.MixY = py;

                    Color target = bitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

                    // 2. Cache TargetColor with Color
                    if (target.A is byte.MaxValue) return this.InkMixer.Cache(target);
                }
            }

            return false;
        }

        private bool Mix(Vector2 position, float opacity)
        {
            // 1. Get Position and Target
            int px = (int)position.X;
            int py = (int)position.Y;
            if (this.BitmapLayer.Contains(px, py) is false) return false;

            if (this.MixX == px && this.MixY == py) return false;
            this.MixX = px;
            this.MixY = py;

            Color target = this.BitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

            // 2. Blend TargetColor with Color
            return this.InkMixer.Mix(target, opacity);
        }
        private bool MixAll(Vector2 position, float opacity)
        {
            foreach (ILayer item in this.ObservableCollection)
            {
                if (item.Type is LayerType.Bitmap is false) continue;

                if (item is BitmapLayer bitmapLayer)
                {
                    // 1. Get Position and Target
                    int px = (int)position.X;
                    int py = (int)position.Y;
                    if (bitmapLayer.Contains(px, py) is false) return false;

                    if (this.MixX == px && this.MixY == py) return false;
                    this.MixX = px;
                    this.MixY = py;

                    Color target = bitmapLayer.GetPixelColor(px, py, BitmapType.Origin);

                    // 2. Blend TargetColor with Color
                    if (target.A is byte.MaxValue) return this.InkMixer.Mix(target, opacity);
                }
            }

            return false;
        }

    }
}