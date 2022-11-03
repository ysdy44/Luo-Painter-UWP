using Luo_Painter.Brushes;
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
    public sealed partial class DrawPage : Page, ILayerManager, IInkParameter
    {

        double DisplacementLiquefactionSize => (float)this.DisplacementLiquefactionSizeSlider.Value;
        double DisplacementLiquefactionPressure => (float)this.DisplacementLiquefactionPressureSlider.Value;

        int DisplacementLiquefactionMode => this.DisplacementLiquefactionModeComboBox.SelectedIndex;

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


        private void DisplacementLiquefaction_Delta()
        {
            if (this.StartingPosition == this.Position) return;

            float radius = (float)this.DisplacementLiquefactionSize;
            float pressure = (float)this.DisplacementLiquefactionPressure / 100;

            this.Displacement.Shade(new PixelShaderEffect(this.DisplacementLiquefactionShaderCodeBytes)
            {
                Source1BorderMode = EffectBorderMode.Hard,
                Source1 = this.Displacement[BitmapType.Source],
                Properties =
                {
                    /// <see cref="DisplacementLiquefactionMode"/> to <see cref="System.Int32"/>
                    ["mode"] =1+this.DisplacementLiquefactionMode,
                    ["amount"] = this.DisplacementLiquefactionAmount,
                    ["pressure"] = pressure,
                    ["radius"] = radius,
                    ["position"] = this.StartingPosition,
                    ["targetPosition"] = this.Position,
                }
            }, RectExtensions.GetRect(this.Position, this.Position, radius));

            Rect region = this.Point.GetRect(this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(radius * 2 * this.Transformer.Scale));
            if (this.CanvasVirtualControl.Size.TryIntersect(ref region))
            {
                this.CanvasVirtualControl.Invalidate(region); // Invalidate
            }

            this.StartingPosition = this.Position;
        }

        private void DisplacementLiquefaction_Complete()
        {
            this.Displacement.RenderThumbnail();
        }

    }
}