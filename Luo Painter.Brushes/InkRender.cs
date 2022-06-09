using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Graphics.Imaging;
using Windows.UI;

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

    public sealed class InkRender
    {

        public ICanvasImage Source => this.Bitmap;
        readonly CanvasRenderTarget Bitmap;

        readonly InkRenderSize SizeInPixels;
        readonly InkRenderSize Size;
        readonly float ScaleForDPI;

        private readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        public InkRender(ICanvasResourceCreatorWithDpi resourceCreator, int width, int height)
        {
            this.Bitmap = new CanvasRenderTarget(resourceCreator, width, height);
            this.SizeInPixels = new InkRenderSize(this.Bitmap.SizeInPixels);
            this.Size = new InkRenderSize(width, height);

            //@DPI
            this.ScaleForDPI = resourceCreator.Dpi / 96;
        }


        // 0 ~ Π
        private double Radian(double value) => System.Math.Clamp(value, 0, 1) * FanKit.Math.Pi;
        // Sin 0 ~ Π ︵
        private double Pressure(double radian) => System.Math.Clamp(System.Math.Sin(radian), 0, 1);
        // Sin 0 ~ 2Π ~
        private double OffsetY(double radian) => (float)System.Math.Clamp(System.Math.Sin(radian + radian), -1, 1);


        public void DrawLine(float strokeWidth, Color color)
        {
            Vector2 position = this.Size.Open;
            float x = this.Size.Open.X + strokeWidth * 10;

            using (CanvasDrawingSession ds = this.Bitmap.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                do
                {
                    // 1. Get Radian
                    double radian = this.Radian(x / this.Size.Width);
                    float offsetY = 20 * (float)this.OffsetY(radian);

                    // 2. Get Position
                    Vector2 targetPosition = new Vector2(x, this.Size.Height / 2 + offsetY);

                    // 3. Draw
                    ds.DrawLine(position, targetPosition, color, strokeWidth, this.CanvasStrokeStyle);
                    position = targetPosition;

                    // 4. Foreach
                    x += strokeWidth * 10;
                } while (x < this.Size.End.X);

                ds.DrawLine(position, this.Size.End, color, strokeWidth, this.CanvasStrokeStyle);
            }
        }


        public void IsometricFillCircle(
            Color color,
            float size = 22f,
            float spacing = 0.25f
            )
        {
            float x = this.Size.Open.X;

            using (CanvasDrawingSession ds = this.Bitmap.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);

                ds.FillCircle(this.Size.Open, size * 0.001f, color);
                ds.FillCircle(this.Size.End, size * 0.001f, color);

                do
                {
                    // 1. Get Radian
                    double radian = this.Radian(x / this.Size.Width);
                    float offsetY = 20 * (float)this.OffsetY(radian);
                    float pressure = (float)this.Pressure(radian);

                    // 2. Get Position
                    Vector2 position = new Vector2(x, this.Size.Height / 2 + offsetY);

                    // 3. Draw
                    float sizePressure = size * pressure + 1;
                    ds.FillCircle(position, sizePressure, color);

                    // 4. Foreach
                    x += sizePressure * spacing;
                } while (x < this.Size.End.X);
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardness(
            byte[] shaderCode,
            Vector4 colorHdr,
            float size = 22f,
            float spacing = 0.25f,
            int hardness = 0
            )
        {
            float x = this.SizeInPixels.Open.X;

            using (CanvasDrawingSession ds = this.Bitmap.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = 1f,
                        ["radius"] = size * 0.001f,
                        ["targetPosition"] = this.SizeInPixels.Open,
                        ["color"] = colorHdr
                    }
                });
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = 1f,
                        ["radius"] = size * 0.001f,
                        ["targetPosition"] = this.SizeInPixels.End,
                        ["color"] = colorHdr
                    }
                });

                do
                {
                    // 1. Get Radian
                    double radian = this.Radian(x / this.SizeInPixels.Width);
                    float offsetY = 20 * (float)this.OffsetY(radian) * this.ScaleForDPI;
                    float pressure = (float)this.Pressure(radian);

                    // 2. Get Position
                    Vector2 position = new Vector2(x, this.SizeInPixels.Height / 2 + offsetY);

                    // 3. Draw
                    float sizePressure = size * pressure / this.ScaleForDPI / this.ScaleForDPI + this.ScaleForDPI;
                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["pressure"] = 1f,
                            ["radius"] = sizePressure,
                            ["targetPosition"] = position,
                            ["color"] = colorHdr
                        }
                    });

                    // 4. Foreach
                    x += sizePressure * spacing;
                } while (x < this.SizeInPixels.End.X);
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardnessWithTexture(
            byte[] shaderCode,
            Vector4 colorHdr,
            CanvasBitmap texture,
            bool rotate,
            float size = 22f,
            float spacing = 0.25f,
            int hardness = 0
            )
        {
            Vector2 position = this.SizeInPixels.Open;
            float x = this.SizeInPixels.Open.X;

            using (CanvasDrawingSession ds = this.Bitmap.CreateDrawingSession())
            {
                //@DPI 
               ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);

                do
                {
                    // 1. Get Radian
                    double radian = this.Radian(x / this.SizeInPixels.Width);
                    float offsetY = 20 * (float)this.OffsetY(radian) * this.ScaleForDPI;
                    float pressure = (float)this.Pressure(radian);

                    // 2. Get Position
                    Vector2 targetPosition = new Vector2(x, this.SizeInPixels.Height / 2 + offsetY);
                    Vector2 normalization = Vector2.Normalize(targetPosition - position);

                    // 3. Draw
                    float sizePressure = size * pressure / this.ScaleForDPI / this.ScaleForDPI + this.ScaleForDPI;
                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Source1 = texture,
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["rotate"] = rotate,
                            ["normalization"] = normalization,
                            ["pressure"] = 1f,
                            ["radius"] = sizePressure,
                            ["targetPosition"] = targetPosition,
                            ["color"] = colorHdr
                        }
                    });
                    position = targetPosition;

                    // 4. Foreach
                    x += sizePressure * spacing;
                } while (x < this.SizeInPixels.End.X);
            }
        }

    }
}