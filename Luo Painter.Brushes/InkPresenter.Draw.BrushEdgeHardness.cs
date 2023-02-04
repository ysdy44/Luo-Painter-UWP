using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardness(CanvasDrawingSession ds, byte[] shaderCode, Vector4 colorHdr, float scaleForDPI)
        {
            float size = this.Size / 24 + 1;
            float spacing = this.Spacing;
            int hardness = (int)this.Hardness;

            float open = this.IgnoreSizePressure ? (size + 10) : 10;
            float end = this.IgnoreSizePressure ? (InkPresenter.Width - size - 10) : (InkPresenter.Width - 10);

            float startingSizePressure = this.IgnoreSizePressure ? (size + 1) : (size * 0.001f + 1);
            float x = open + startingSizePressure * spacing;

            if (this.IgnoreFlowPressure)
            {
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.Flow,
                        ["radius"] = startingSizePressure * scaleForDPI,
                        ["targetPosition"] = new Vector2(open, InkPresenter.HeightHalf) * scaleForDPI,
                        ["color"] = colorHdr
                    }
                });
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.Flow,
                        ["radius"] = startingSizePressure * scaleForDPI,
                        ["targetPosition"] = new Vector2(end, InkPresenter.HeightHalf) * scaleForDPI,
                        ["color"] = colorHdr
                    }
                });
            }

            do
            {
                // 1. Get Radian
                double radian = this.Radian((x - open) / (end - open));
                float offsetY = 20 * (float)this.OffsetY(radian);
                float pressure = (float)this.Pressure(radian);

                // 2. Get Position
                Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                // 3. Draw
                float sizePressure = this.IgnoreSizePressure ? (size + 1) : (size * pressure * pressure + 1);
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.IgnoreFlowPressure ? this.Flow : this.Flow * pressure,
                        ["radius"] = sizePressure * scaleForDPI,
                        ["targetPosition"] = position * scaleForDPI,
                        ["color"] = colorHdr
                    }
                });

                // 4. Foreach
                x += sizePressure * spacing;
            } while (x < end);
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardnessWithTexture(CanvasDrawingSession ds, byte[] shaderCode, Vector4 colorHdr, float scaleForDPI)
        {
            float size = this.Size / 24 + 1;
            float spacing = this.Spacing;
            int hardness = (int)this.Hardness;

            float open = this.IgnoreSizePressure ? (size + 10) : 10;
            float end = this.IgnoreSizePressure ? (InkPresenter.Width - size - 10) : (InkPresenter.Width - 10);

            Vector2 position = new Vector2(open, InkPresenter.HeightHalf);
            float startingSizePressure = this.IgnoreSizePressure ? (size + 1) : (size * 0.001f + 1);
            float x = open + startingSizePressure * spacing;

            do
            {
                // 1. Get Radian
                double radian = this.Radian((x - open) / (end - open));
                float offsetY = 20 * (float)this.OffsetY(radian);
                float pressure = (float)this.Pressure(radian);

                // 2. Get Position
                Vector2 targetPosition = new Vector2(x, InkPresenter.Height / 2 + offsetY);
                Vector2 normalization = Vector2.Normalize(targetPosition - position);

                // 3. Draw
                float sizePressure = this.IgnoreSizePressure ? (size + 1) : (size * pressure * pressure + 1);
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = this.ShapeSource,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = this.Rotate,
                        ["normalization"] = normalization,
                        ["pressure"] = this.IgnoreFlowPressure ? this.Flow : this.Flow * pressure,
                        ["radius"] = sizePressure * scaleForDPI,
                        ["targetPosition"] = position * scaleForDPI,
                        ["color"] = colorHdr
                    }
                });
                position = targetPosition;

                // 4. Foreach
                x += sizePressure * spacing;
            } while (x < end);
        }

    }
}