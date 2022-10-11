using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    internal struct InkRenderSize
    {
        public readonly int Width;
        public readonly int Height;
        public readonly Vector2 Open;
        public readonly Vector2 End;
        public InkRenderSize(BitmapSize size) : this((int)size.Width, (int)size.Height) { }
        public InkRenderSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Open = new Vector2(10, height / 2);
            this.End = new Vector2(width - 10, height / 2);
        }
    }

    public sealed partial class InkPresenter
    {

        //@Const
        public const int Width = 320;
        public const int Height = 120;

        private readonly InkRenderSize RenderSize = new InkRenderSize(InkPresenter.Width, InkPresenter.Height);
        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };


        // 0 ~ Π
        private double Radian(double value) => System.Math.Clamp(value, 0, 1) * FanKit.Math.Pi;
        // Sin 0 ~ Π ︵
        private double Pressure(double radian) => System.Math.Clamp(System.Math.Sin(radian), 0, 1);
        // Sin 0 ~ 2Π ~
        private double OffsetY(double radian) => (float)System.Math.Clamp(System.Math.Sin(radian + radian), -1, 1);


        public void DrawLine(CanvasDrawingSession ds, Color color)
        {
            float strokeWidth = this.Size / 24 + 1;

            Vector2 position = this.RenderSize.Open;
            float x = this.RenderSize.Open.X + strokeWidth * 10;
            float xLength = this.RenderSize.End.X;

            do
            {
                // 1. Get Radian
                double radian = this.Radian(x / this.RenderSize.Width);
                float offsetY = 20 * (float)this.OffsetY(radian);

                // 2. Get Position
                Vector2 targetPosition = new Vector2(x, this.RenderSize.Height / 2 + offsetY);

                // 3. Draw
                ds.DrawLine(position, targetPosition, color, strokeWidth, this.CanvasStrokeStyle);
                position = targetPosition;

                // 4. Foreach
                x += strokeWidth * 10;
            } while (x < xLength);

            ds.DrawLine(position, this.RenderSize.End, color, strokeWidth, this.CanvasStrokeStyle);
        }


        public void IsometricFillCircle(CanvasDrawingSession ds, Color color, bool ignoreSpacing)
        {
            float size = this.Size / 24 + 1;
            float spacing = ignoreSpacing ? 0.25f : this.Spacing;

            float x = this.RenderSize.Open.X;
            float xLength = this.RenderSize.End.X;

            float startingSize = size * 0.001f;
            switch (this.Shape)
            {
                case PenTipShape.Circle:
                    if (this.IsStroke)
                    {
                        ds.DrawCircle(this.RenderSize.Open, startingSize, color);
                        ds.DrawCircle(this.RenderSize.End, startingSize, color);
                    }
                    else
                    {
                        ds.FillCircle(this.RenderSize.Open, startingSize, color);
                        ds.FillCircle(this.RenderSize.End, startingSize, color);
                    }
                    break;
                case PenTipShape.Rectangle:
                    if (this.IsStroke)
                    {
                        ds.DrawRectangle(this.RenderSize.Open.X - startingSize, this.RenderSize.Open.Y - startingSize, startingSize + startingSize, startingSize + startingSize, color);
                        ds.DrawRectangle(this.RenderSize.End.X - startingSize, this.RenderSize.End.Y - startingSize, startingSize + startingSize, startingSize + startingSize, color);
                    }
                    else
                    {
                        ds.FillRectangle(this.RenderSize.Open.X - startingSize, this.RenderSize.Open.Y - startingSize, startingSize + startingSize, startingSize + startingSize, color);
                        ds.FillRectangle(this.RenderSize.End.X - startingSize, this.RenderSize.End.Y - startingSize, startingSize + startingSize, startingSize + startingSize, color);
                    }
                    break;
                default:
                    break;
            }

            switch (this.Shape)
            {
                case PenTipShape.Circle:
                    do
                    {
                        // 1. Get Radian
                        double radian = this.Radian(x / this.RenderSize.Width);
                        float offsetY = 20 * (float)this.OffsetY(radian);
                        float pressure = (float)this.Pressure(radian);

                        // 2. Get Position
                        Vector2 position = new Vector2(x, this.RenderSize.Height / 2 + offsetY);

                        // 3. Draw
                        float sizePressure = size * pressure * pressure + 1;
                        if (this.IsStroke)
                            ds.DrawCircle(position, sizePressure, color);
                        else
                            ds.FillCircle(position, sizePressure, color);

                        // 4. Foreach
                        x += sizePressure * spacing;
                    } while (x < xLength);
                    break;
                case PenTipShape.Rectangle:
                    do
                    {
                        // 1. Get Radian
                        double radian = this.Radian(x / this.RenderSize.Width);
                        float offsetY = 20 * (float)this.OffsetY(radian);
                        float pressure = (float)this.Pressure(radian);

                        // 2. Get Position
                        Vector2 position = new Vector2(x, this.RenderSize.Height / 2 + offsetY);

                        // 3. Draw
                        float sizePressure = size * pressure * pressure + 1;
                        if (this.IsStroke)
                            ds.DrawRectangle(position.X - sizePressure, position.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);
                        else
                            ds.FillRectangle(position.X - sizePressure, position.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);

                        // 4. Foreach
                        x += sizePressure * spacing;
                    } while (x < xLength);
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
            float size = this.Size;
            float spacing = this.Spacing;
            int hardness = (int)this.Hardness;

            float x = this.RenderSize.Open.X * scaleForDPI;
            float xLength = this.RenderSize.End.X * scaleForDPI;

            float startingSize = size * 0.001f;
            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Properties =
                {
                    ["hardness"] = hardness,
                    ["pressure"] = this.Flow,
                    ["radius"] = startingSize,
                    ["targetPosition"] = this.RenderSize.Open * scaleForDPI,
                    ["color"] = colorHdr
                }
            });
            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Properties =
                {
                    ["hardness"] = hardness,
                    ["pressure"] = this.Flow,
                    ["radius"] = startingSize,
                    ["targetPosition"] = this.RenderSize.End * scaleForDPI,
                    ["color"] = colorHdr
                }
            });

            do
            {
                // 1. Get Radian
                double radian = this.Radian(x / (this.RenderSize.Width * scaleForDPI));
                float offsetY = 20 * (float)this.OffsetY(radian) * scaleForDPI;
                float pressure = (float)this.Pressure(radian);

                // 2. Get Position
                Vector2 position = new Vector2(x, this.RenderSize.Height * scaleForDPI / 2 + offsetY);

                // 3. Draw
                float sizePressure = size * pressure * pressure / scaleForDPI / scaleForDPI + scaleForDPI;
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.Flow,
                        ["radius"] = sizePressure,
                        ["targetPosition"] = position,
                        ["color"] = colorHdr
                    }
                });

                // 4. Foreach
                x += sizePressure * spacing;
            } while (x < xLength);
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardnessWithTexture(CanvasDrawingSession ds, byte[] shaderCode, Vector4 colorHdr, float scaleForDPI)
        {
            float size = this.Size;
            float spacing = this.Spacing;
            int hardness = (int)this.Hardness;

            Vector2 position = this.RenderSize.Open * scaleForDPI;
            float x = this.RenderSize.Open.X * scaleForDPI;
            float xLength = this.RenderSize.End.X * scaleForDPI;

            do
            {
                // 1. Get Radian
                double radian = this.Radian(x / (this.RenderSize.Width * scaleForDPI));
                float offsetY = 20 * (float)this.OffsetY(radian) * scaleForDPI;
                float pressure = (float)this.Pressure(radian);

                // 2. Get Position
                Vector2 targetPosition = new Vector2(x, this.RenderSize.Height * scaleForDPI / 2 + offsetY);
                Vector2 normalization = Vector2.Normalize(targetPosition - position);

                // 3. Draw
                float sizePressure = size * pressure * pressure / scaleForDPI / scaleForDPI + scaleForDPI;
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = this.Mask,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = this.Rotate,
                        ["normalization"] = normalization,
                        ["pressure"] = this.Flow,
                        ["radius"] = sizePressure,
                        ["targetPosition"] = targetPosition,
                        ["color"] = colorHdr
                    }
                });
                position = targetPosition;

                // 4. Foreach
                x += sizePressure * spacing;
            } while (x < xLength);
        }

    }
}