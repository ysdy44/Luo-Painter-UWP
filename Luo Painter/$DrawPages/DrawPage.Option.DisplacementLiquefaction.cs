using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Shaders;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page, ILayerManager
    {

        float DisplacementLiquefactionAmount = 512;

        private void SetDisplacementLiquefaction()
        {
            this.DisplacementLiquefactionAmount = this.Displacement.Center.Length();
            this.Displacement.ClearDisplacement();
            this.Displacement.RenderThumbnail();
        }

        private void DrawDisplacementLiquefaction(CanvasControl sender, CanvasDrawingSession ds)
        {
        }

        private ICanvasImage GetDisplacementLiquefactionPreview(ICanvasImage image)
        {
            return new DisplacementMapEffect
            {
                XChannelSelect = EffectChannelSelect.Red,
                YChannelSelect = EffectChannelSelect.Green,
                Amount = this.DisplacementLiquefactionAmount,
                Source = image,
                Displacement = new GaussianBlurEffect
                {
                    BlurAmount = 16,
                    Source = new BorderEffect
                    {
                        ExtendX = CanvasEdgeBehavior.Clamp,
                        ExtendY = CanvasEdgeBehavior.Clamp,
                        Source = this.Displacement[BitmapType.Source],
                    }
                }
            };
        }


        private void DisplacementLiquefaction_Start(Vector2 point)
        {
            this.Position = this.ToPosition(point);
        }

        private void DisplacementLiquefaction_Delta(Vector2 point)
        {
            Vector2 position = this.ToPosition(point);
            if (this.Position == position) return;

            float radius = (float)this.AppBar.DisplacementLiquefactionSize;
            float pressure = (float)this.AppBar.DisplacementLiquefactionPressure / 100;

            this.Displacement.Shade(new PixelShaderEffect(this.DisplacementLiquefactionShaderCodeBytes)
            {
                Source1BorderMode = EffectBorderMode.Hard,
                Source1 = this.Displacement[BitmapType.Source],
                Properties =
                {
                    /// <see cref="DisplacementLiquefactionMode"/> to <see cref="System.Int32"/>
                    ["mode"] =1+this.AppBar.DisplacementLiquefactionMode,
                    ["amount"] = this.DisplacementLiquefactionAmount,
                    ["pressure"] = pressure,
                    ["radius"] = radius,
                    ["position"] = this.Position,
                    ["targetPosition"] = position,
                }
            }, RectExtensions.GetRect(this.Position, position, radius));

            Rect region = point.GetRect(this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(radius * 2 * this.Transformer.Scale));
            if (this.CanvasVirtualControl.Size.TryIntersect(ref region))
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
            }

            this.Position = position;
        }

        private void DisplacementLiquefaction_Complete(Vector2 point)
        {
            this.Displacement.RenderThumbnail();
        }

    }
}