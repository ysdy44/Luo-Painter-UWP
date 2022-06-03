using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Graphics.Effects;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        //@Static
        public static readonly Vector4 DisplacementColor = new Vector4(0.5f, 0.5f, 1, 1);
        public static readonly Vector4 DodgerBlue = new Vector4(0.11764705882352941176470588235294f, 0.56470588235294117647058823529412f, 1, 1);
        private static readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };


        public void ClearDisplacement()
        {
            using (CanvasDrawingSession ds = this.OriginRenderTarget.CreateDrawingSession())
            {
                ds.Clear(BitmapLayer.DisplacementColor);
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                ds.Clear(BitmapLayer.DisplacementColor);
            }
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                ds.Clear(BitmapLayer.DisplacementColor);
            }
        }


        public void Shade(IGraphicsEffectSource source, Windows.Foundation.Rect rect)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = source
                });
            }
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new CropEffect
                {
                    SourceRectangle = rect,
                    Source = this.TempRenderTarget
                });
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void IsometricShape(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, byte[] shaderCode, int hardness, Vector4 colorHdr, BitmapType type)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1BorderMode = EffectBorderMode.Hard,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = 1f,
                        ["radius"] = size * pressure,
                        ["targetPosition"] = position,
                        ["color"] = colorHdr
                    }
                });

                float length = Vector2.Distance(position, targetPosition);
                if (length > size * targetPressure / 2)
                {
                    float spane = size / 2 / length * pressure;
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        float e = pressure * i + (1 - i) * targetPressure;
                        ds.DrawImage(new PixelShaderEffect(shaderCode)
                        {
                            Source1BorderMode = EffectBorderMode.Hard,
                            Properties =
                            {
                                ["hardness"] = hardness,
                                ["pressure"] = 1f,
                                ["radius"] = size * e,
                                ["targetPosition"] = p,
                                ["color"] = colorHdr
                           }
                        });
                    }
                }
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void IsometricShape(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, byte[] shaderCode, CanvasBitmap texture, int hardness, bool rotate, Vector4 colorHdr, BitmapType type)
        {
            Vector2 vector = targetPosition - position;
            if (double.IsNaN(vector.X)) return;
            if (double.IsNaN(vector.Y)) return;
            Vector2 normalization = Vector2.Normalize(vector);

            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1BorderMode = EffectBorderMode.Hard,
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = normalization,
                        ["pressure"] = 1f,
                        ["radius"] = size * pressure,
                        ["targetPosition"] = position,
                        ["color"] = colorHdr
                    }
                });

                float length = Vector2.Distance(position, targetPosition);
                if (length > size * targetPressure / 2)
                {
                    float spane = size / 2 / length * pressure;
                    for (float i = spane; i < 1f; i += spane)
                    {
                        Vector2 p = position * i + (1 - i) * targetPosition;
                        float e = pressure * i + (1 - i) * targetPressure;
                        ds.DrawImage(new PixelShaderEffect(shaderCode)
                        {
                            Source1BorderMode = EffectBorderMode.Hard,
                            Source1 = texture,
                            Properties =
                            {
                                ["hardness"] = hardness,
                                ["rotate"] = rotate,
                                ["normalization"] = normalization,
                                ["pressure"] = 1f,
                                ["radius"] = size * e,
                                ["targetPosition"] = p,
                                ["color"] = colorHdr
                           }
                        });
                    }
                }
            }
        }


        public void IsometricFillCircle(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color)
        {
            ds.FillCircle(position, size * targetPressure, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size * targetPressure / 2)
            {
                float spane = size / 2 / length * pressure;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    float e = pressure * i + (1 - i) * targetPressure;
                    ds.FillCircle(p, size * e, color);
                }
            }
        }


        public void IsometricFillCircle(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size, Color color, BitmapType type)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                this.IsometricFillCircle(ds, position, targetPosition, pressure, targetPressure, size, color);
            }
        }


        public void ErasingDry(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Blend = CanvasBlend.Copy;

                this.IsometricFillCircle(ds, position, targetPosition, pressure, targetPressure, size, Colors.Transparent);
            }
        }
        public void ErasingWet(Vector2 position, Vector2 targetPosition, float pressure, float targetPressure, float size)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                this.IsometricFillCircle(ds, position, targetPosition, pressure, targetPressure, size, Colors.White);
            }
        }


        public void DrawLine(Vector2 position, Vector2 targetPosition, Color color, float strokeWidth, BitmapType type)
        {
            using (CanvasDrawingSession ds = this.CreateDrawingSession(type))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawLine(position, targetPosition, color, strokeWidth, BitmapLayer.CanvasStrokeStyle);
            }
        }


        public void FillCircleFlipX(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(width - position.X, position.Y, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(width - p.X, p.Y, size, color);
                }
            }
        }

        public void FillCircleFlipY(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float height)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(position.X, height - position.Y, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(p.X, height - p.Y, size, color);
                }
            }
        }

        public void FillCircleTwo(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width, float height)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(width - position.X, height - position.Y, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(width - p.X, height - p.Y, size, color);
                }
            }
        }

        public void FillCircleFour(CanvasDrawingSession ds, Vector2 position, Vector2 targetPosition, float size, Color color, float width, float height, Vector2 center)
        {
            //@DPI 
            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

            ds.FillCircle(position, size, color);
            ds.FillCircle(width - position.X, height - position.Y, size, color);

            Vector2 vector = position - center;
            ds.FillCircle(center.X - vector.Y, center.Y + vector.X, size, color);
            ds.FillCircle(center.X + vector.Y, center.Y - vector.X, size, color);

            float length = Vector2.Distance(position, targetPosition);
            if (length > size / 2)
            {
                float spane = size / 2 / length;
                for (float i = spane; i < 1f; i += spane)
                {
                    Vector2 p = position * i + (1 - i) * targetPosition;
                    ds.FillCircle(p, size, color);
                    ds.FillCircle(width - p.X, height - p.Y, size, color);

                    Vector2 v = p - center;
                    ds.FillCircle(center.X - v.Y, center.Y + v.X, size, color);
                    ds.FillCircle(center.X + v.Y, center.Y - v.X, size, color);
                }
            }
        }

    }
}