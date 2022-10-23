using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes<float>
    {

        //@Const
        public const int Width = 320;
        public const int Height = 120;
        public const int HeightHalf = 60;

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        // 0 ~ Π
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Radian(double value) => System.Math.Clamp(value, 0, 1) * FanKit.Math.Pi;
        // Sin 0 ~ Π ︵
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Pressure(double radian) => System.Math.Clamp(System.Math.Sin(radian), 0, 1);
        // Sin 0 ~ 2Π ~
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double OffsetY(double radian) => (float)System.Math.Clamp(System.Math.Sin(radian + radian), -1, 1);


        public void IsometricTip(CanvasDrawingSession ds, Color color, bool ignoreSpacing)
        {
            float size = this.Size / 24 + 1;
            float spacing = ignoreSpacing ? 0.25f : this.Spacing;

            float open = this.IgnoreSizePressure ? (size + 10) : 10;
            float end = this.IgnoreSizePressure ? (InkPresenter.Width - size - 10) : (InkPresenter.Width - 10);

            float startingSizePressure = this.IgnoreSizePressure ? (size + 1) : (size * 0.001f + 1);
            float x = open + startingSizePressure * spacing;

            switch (this.Tip)
            {
                case PenTipShape.Circle:
                    if (this.IsStroke)
                    {
                        ds.DrawCircle(open, InkPresenter.HeightHalf, startingSizePressure, color);
                        ds.DrawCircle(end, InkPresenter.HeightHalf, startingSizePressure, color);
                    }
                    else
                    {
                        ds.FillCircle(open, InkPresenter.HeightHalf, startingSizePressure, color);
                        ds.FillCircle(end, InkPresenter.HeightHalf, startingSizePressure, color);
                    }
                    break;
                case PenTipShape.Rectangle:
                    if (this.IsStroke)
                    {
                        ds.DrawRectangle(open - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                        ds.DrawRectangle(end - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                    }
                    else
                    {
                        ds.FillRectangle(open - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                        ds.FillRectangle(end - startingSizePressure, InkPresenter.HeightHalf - startingSizePressure, startingSizePressure + startingSizePressure, startingSizePressure + startingSizePressure, color);
                    }
                    break;
                default:
                    break;
            }

            switch (this.Tip)
            {
                case PenTipShape.Circle:
                    do
                    {
                        // 1. Get Radian
                        double radian = this.Radian((x - open) / (end - open));
                        float offsetY = 20 * (float)this.OffsetY(radian);
                        float pressure = this.IgnoreSizePressure ? 1 : (float)this.Pressure(radian);

                        // 2. Get Position
                        Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                        // 3. Draw
                        float sizePressure = this.IgnoreSizePressure ? (size + 1) : (size * pressure * pressure + 1);
                        if (this.IsStroke)
                            ds.DrawCircle(position, sizePressure, color);
                        else
                            ds.FillCircle(position, sizePressure, color);

                        // 4. Foreach
                        x += sizePressure * spacing;
                    } while (x < end);
                    break;
                case PenTipShape.Rectangle:
                    do
                    {
                        // 1. Get Radian
                        double radian = this.Radian(x / InkPresenter.Width);
                        float offsetY = 20 * (float)this.OffsetY(radian);
                        float pressure = this.IgnoreSizePressure ? 1 : (float)this.Pressure(radian);

                        // 2. Get Position
                        Vector2 position = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                        // 3. Draw
                        float sizePressure = this.IgnoreSizePressure ? (size + 1) : (size * pressure * pressure + 1);
                        if (this.IsStroke)
                            ds.DrawRectangle(position.X - sizePressure, position.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);
                        else
                            ds.FillRectangle(position.X - sizePressure, position.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);

                        // 4. Foreach
                        x += sizePressure * spacing;
                    } while (x < end);
                    break;
                default:
                    break;
            }
        }


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


        public void DrawLine(CanvasDrawingSession ds, Color color)
        {
            float strokeWidth = this.Size / 24 + 1;
            float spacing = this.Spacing;

            float open = strokeWidth + 10;
            float end = InkPresenter.Width - strokeWidth - 10;

            Vector2 position = new Vector2(open, InkPresenter.HeightHalf);
            float startingSizePressure = strokeWidth + 1;
            float x = open + startingSizePressure * spacing;

            do
            {
                // 1. Get Radian
                double radian = this.Radian((x - open) / (end - open));
                float offsetY = 20 * (float)this.OffsetY(radian);

                // 2. Get Position
                Vector2 targetPosition = new Vector2(x, InkPresenter.Height / 2 + offsetY);

                // 3. Draw
                ds.DrawLine(position, targetPosition, color, strokeWidth, this.CanvasStrokeStyle);
                position = targetPosition;

                // 4. Foreach
                x += strokeWidth * spacing * 12;
            } while (x < end);

            ds.DrawLine(position.X, position.Y, end, InkPresenter.HeightHalf, color, strokeWidth, this.CanvasStrokeStyle);
        }

    }
}