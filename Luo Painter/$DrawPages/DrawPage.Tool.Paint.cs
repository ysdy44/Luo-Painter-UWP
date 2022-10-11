using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        int MixX = -1;
        int MixY = -1;

        private void Paint_Start()
        {
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

            if (this.InkType.HasFlag(InkType.Mix)) this.CacheMix(this.Position);

            this.CanvasVirtualControl.Invalidate(); // Invalidate
        }

        private void Paint_Delta(Vector2 position, Vector2 point, float pressure)
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            StrokeSegment segment = new StrokeSegment(this.Position, position, this.Pressure, pressure, this.InkPresenter.Size, this.InkPresenter.Spacing);

            if (segment.InRadius()) return;
            if (this.InkType.HasFlag(InkType.Mask) && segment.IsNaN()) return; // Mask without NaN
            if (this.InkType.HasFlag(InkType.Mix)) this.Mix(position, this.InkPresenter.Opacity);

            Rect rect = position.GetRect(this.InkPresenter.Size);
            this.BitmapLayer.Hit(rect);
            this.Paint(segment);

            Rect region = RectExtensions.GetRect(this.Point, point, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(this.InkPresenter.Size * this.Transformer.Scale));
            if (this.CanvasVirtualControl.Size.TryIntersect(ref region))
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
            }

            this.Position = position;
            this.Point = point;
            this.Pressure = pressure;
        }

        private void Paint_Complete()
        {
            if (this.InkType == default) return;
            if (this.BitmapLayer is null) return;

            this.Paint();
            this.BitmapLayer.Clear(Colors.Transparent, BitmapType.Temp);

            // History
            int removes = this.History.Push(this.BitmapLayer.GetBitmapHistory());
            this.BitmapLayer.Flush();
            this.BitmapLayer.RenderThumbnail();

            this.BitmapLayer = null;
            this.CanvasVirtualControl.Invalidate(); // Invalidate

            this.UndoButton.IsEnabled = this.History.CanUndo;
            this.RedoButton.IsEnabled = this.History.CanRedo;
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