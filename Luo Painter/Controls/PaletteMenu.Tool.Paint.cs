using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;

namespace Luo_Painter.Controls
{
    public sealed partial class PaletteMenu : Expander, IInkParameter
    {

        int MixX = -1;
        int MixY = -1;

        private void Paint_Start()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            if (this.InkType.HasFlag(InkType.Mix)) this.CacheMix(this.StartingPosition);
            if (this.InkType.HasFlag(InkType.Shape) && this.InkPresenter.Rotate) return; // Shape without NaN
            if (this.InkType.HasFlag(InkType.Liquefy)) return; // Liquefy without NaN

            StrokeCap cap = new StrokeCap(this.StartingPosition, this.StartingPressure, this.InkPresenter.Size);

            //@Task
            if (false)
                this.PaintCapAsync(cap);
            else
                Task.Run(() => this.PaintCapAsync(cap));
        }
        private void Paint_Delta()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            StrokeSegment segment = new StrokeSegment(this.StartingPosition, this.Position, this.StartingPressure, this.Pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

            if (segment.InRadius) return;
            if (segment.IsNaN)
            {
                if (this.InkType.HasFlag(InkType.Shape) && this.InkPresenter.Rotate) return; // Shape without NaN
                if (this.InkType.HasFlag(InkType.Liquefy)) return; // Liquefy without NaN
            }
            if (this.InkType.HasFlag(InkType.Mix)) this.Mix(this.Position, this.InkPresenter.Opacity);

            //@Task
            if (false)
                this.PaintSegmentAsync(segment);
            else
                Task.Run(() => this.PaintSegmentAsync(segment));

            this.StartingPosition = this.Position;
            this.StartingPressure = this.Pressure;
        }
        private void Paint_Complete()
        {
            if (this.CanvasControl.ReadyToDraw is false) return;
            if (this.InkType == default) return;

            //@Task
            if (false)
                this.PaintHistoryAsync();
            else
                Task.Run(this.PaintHistoryAsync);
        }


        private void PaintCapAsync(StrokeCap cap)
        {
            //@Task
            lock (this.Locker)
            {
                this.PaintCap(cap);
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                using (ds.CreateLayer(1f, cap.Bounds))
                {
                    this.InkPresenter.Preview(ds, this.InkType, this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]);
                }

                this.CanvasControl.Invalidate(); // Invalidate
            }
        }
        private void PaintSegmentAsync(StrokeSegment segment)
        {
            //@Task
            lock (this.Locker)
            {
                this.PaintSegment(segment);
                using (CanvasDrawingSession ds = this.BitmapLayer.CreateDrawingSession())
                using (ds.CreateLayer(1f, segment.Bounds))
                {
                    this.InkPresenter.Preview(ds, this.InkType, this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp]);
                }

                this.CanvasControl.Invalidate(); // Invalidate
            }
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

    }
}