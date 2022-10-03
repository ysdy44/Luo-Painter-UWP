using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        int MixX = -1;
        int MixY = -1;

        //@Task
        readonly object Locker = new object();

        private void Paint_Start()
        {
            if (this.InkType.HasFlag(InkType.Mix)) this.CacheMix(this.StartingPosition);

            this.CanvasControl.Invalidate(); // Invalidate
        }
        private void Paint_Delta()
        {
            if (this.InkType == default) return;

            float length = Vector2.Distance(this.StartingPosition, this.Position);

            float sizePressure = System.Math.Max(1, this.InkPresenter.Size * this.Pressure);
            float distance = this.InkPresenter.Spacing * sizePressure; // distance is spacingSizePressure

            if (distance > length) return;
            if (this.InkType.HasFlag(InkType.Mix)) this.Mix(this.Position, this.InkPresenter.Opacity);

            Stroke stroke = new Stroke
            {
                StartingPosition = this.StartingPosition,
                Position = this.Position,
                StartingPressure = this.StartingPressure,
                Pressure = this.Pressure,
            };

            //@Task
            Task.Run(() =>
            {
                lock (this.Locker) /// Locker for <see cref="CanvasDrawingSession"/>
                {
                    if (this.Paint(stroke) is false) return;
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            });

            this.StartingPosition = this.Position;
            this.StartingPressure = this.Pressure;
        }

        private void Paint_Complete()
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            //@Task
            Task.Run(() =>
            {
                lock (this.Locker) /// Locker for <see cref="CanvasDrawingSession"/>
                {
                    if (this.Paint() is false) return;
                    this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

                    // History
                    this.BitmapLayer.Flush();

                    this.CanvasControl.Invalidate(); // Invalidate
                }
            });
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