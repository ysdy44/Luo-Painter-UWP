using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        int MixX = -1;
        int MixY = -1;


        private void Paint_Start()
        {
            if (this.InkType.HasFlag(InkType.Mix)) this.CacheMix(this.Position);

            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void Paint_Delta(Vector2 position, float pressure)
        {
            if (this.InkType == default) return;

            if (this.Paint(position, pressure) is false) return;
            if (this.InkType.HasFlag(InkType.Mix)) this.Mix(position, this.InkPresenter.Opacity);

            this.CanvasControl.Invalidate(); // Invalidate

            this.Position = position;
            this.Pressure = pressure;
        }

        private void Paint_Complete()
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            if (this.Paint() is false) return;
            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

            // History
            this.BitmapLayer.Flush();
   
            this.CanvasControl.Invalidate(); // Invalidate
        }


        private bool Paint()
        {
            if (this.InkType.HasFlag(InkType.Dry)) return true;
            else if (this.InkType.HasFlag(InkType.Wet)) { this.BitmapLayer.Draw(this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp])); return true; }
            else if (this.InkType.HasFlag(InkType.WetBlur)) { this.BitmapLayer.Draw(this.InkPresenter.GetBlur(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp])); return true; }
            else if (this.InkType.HasFlag(InkType.WetMosaic)) { this.BitmapLayer.Draw(this.InkPresenter.GetMosaic(this.BitmapLayer[BitmapType.Origin], this.BitmapLayer[BitmapType.Temp])); return true; }
            else if (this.InkType.HasFlag(InkType.WetComposite)) { this.BitmapLayer.DrawCopy(this.InkPresenter.GetPreview(this.InkType, this.BitmapLayer[BitmapType.Origin], this.InkPresenter.GetWet(this.InkType, this.BitmapLayer[BitmapType.Temp]))); return true; }
            else return false;
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