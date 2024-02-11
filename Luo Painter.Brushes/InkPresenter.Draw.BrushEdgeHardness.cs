using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Brushes
{
    partial class InkPresenter
    {

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardness(CanvasDrawingSession ds, byte[] shaderCode, Vector4 colorHdr, float scaleForDPI)
        {
            float size = this.Size / 24 + 1;
            float spacing = this.Spacing;
            int hardness = (int)this.Hardness;

            float open = 10 + this.GetSizePressed(0.001f) * size;
            float end = InkPresenter.Width - 10 - this.GetSizePressed(0.001f) * size;

            float startingSizePressure = this.GetSizePressed(0.001f) * size + 1;
            float x = open + startingSizePressure * spacing;

            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Properties =
                {
                    ["hardness"] = hardness,
                    ["pressure"] = this.GetFlowPressed(0.001f) * this.Flow,
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
                    ["pressure"] = this.GetFlowPressed(0.001f) * this.Flow,
                    ["radius"] = startingSizePressure * scaleForDPI,
                    ["targetPosition"] = new Vector2(end, InkPresenter.HeightHalf) * scaleForDPI,
                    ["color"] = colorHdr
                }
            });

            do
            {
                // 1. Get Radian
                double radian = this.Radian((x - open) / (end - open));
                float offsetY = 20 * (float)this.OffsetY(radian);
                float pressure = (float)this.Pressure(radian);

                // 2. Get Position
                Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                // 3. Draw
                float sizePressed = this.GetSizePressed(pressure) * size + 1;
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.GetFlowPressed(pressure) * this.Flow,
                        ["radius"] = sizePressed * scaleForDPI,
                        ["targetPosition"] = position * scaleForDPI,
                        ["color"] = colorHdr
                    }
                });

                // 4. Foreach
                x += sizePressed * spacing;
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

            float open = 10 + this.GetSizePressed(0.001f) * size;
            float end = InkPresenter.Width - 10 - this.GetSizePressed(0.001f) * size;

            Vector2 position = new Vector2(open, InkPresenter.HeightHalf);
            float startingSizePressure = this.GetSizePressed(0.001f) * size + 1;
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
                float sizePressed = this.GetSizePressed(pressure) * size + 1;
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = this.ShapeSource,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = this.Rotate,
                        ["normalization"] = normalization,
                        ["pressure"] = this.GetFlowPressed(pressure) * this.Flow,
                        ["radius"] = sizePressed * scaleForDPI,
                        ["targetPosition"] = position * scaleForDPI,
                        ["color"] = colorHdr
                    }
                });
                position = targetPosition;

                // 4. Foreach
                x += sizePressed * spacing;
            } while (x < end);
        }

    }
}