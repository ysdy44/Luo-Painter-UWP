using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Shaders;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Foundation;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        float DisplacementLiquefactionSize => (float)this.DisplacementLiquefactionSizeSlider.Value;
        float DisplacementLiquefactionPressure => (float)this.DisplacementLiquefactionPressureSlider.Value;

        int DisplacementLiquefactionMode => this.DisplacementLiquefactionModeComboBox.SelectedIndex;

        float DisplacementLiquefactionAmount = 512;

        public void ConstructDisplacementLiquefaction()
        {
            this.DisplacementLiquefactionSizeSlider.Click += (s, e) => this.NumberShowAt(this.DisplacementLiquefactionSizeSlider, NumberPickerMode.DisplacementLiquefactionSizeSlider);
            this.DisplacementLiquefactionPressureSlider.Click += (s, e) => this.NumberShowAt(this.DisplacementLiquefactionPressureSlider, NumberPickerMode.DisplacementLiquefactionPressureSlider);
        }

        private void ResetDisplacementLiquefaction()
        {
            this.DisplacementLiquefactionAmount = this.Displacement.Center.Length();
            this.Displacement.ClearDisplacement();
            this.Displacement.RenderThumbnail();
        }

        private void DisplacementLiquefaction_Delta()
        {
            if (this.StartingPosition == this.Position) return;

            float radius = this.DisplacementLiquefactionSize;
            float pressure = this.DisplacementLiquefactionPressure / 100;

            this.Displacement.Shade(new PixelShaderEffect(this.DisplacementLiquefactionShaderCodeBytes)
            {
                Source1BorderMode = EffectBorderMode.Soft,
                Source1 = this.Displacement[BitmapType.Source],
                Properties =
                {
                    /// <see cref="Shaders.DisplacementLiquefactionMode"/> to <see cref="System.Int32"/>
                    ["mode"] =1+this.DisplacementLiquefactionMode,
                    ["amount"] = this.DisplacementLiquefactionAmount,
                    ["pressure"] = pressure, //
                    ["radius"] = radius,
                    ["position"] = this.StartingPosition,
                    ["targetPosition"] = this.Position,
                }
            }, RectExtensions.GetRect(this.Position, radius));

            Rect? region = RectExtensions.TryGetRect(this.Point, this.CanvasVirtualControl.Size, this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(radius * 2 * this.Transformer.Scale));
            this.StartingPosition = this.Position;

            if (region.HasValue)
                this.CanvasVirtualControl.Invalidate(region.Value); // Invalidate
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private void DisplacementLiquefaction_Complete()
        {
            this.Displacement.RenderThumbnail();
        }

    }
}