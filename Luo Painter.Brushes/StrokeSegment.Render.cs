using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;

namespace Luo_Painter.Brushes
{
    public partial struct StrokeSegment
    {

        public static readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        public void IsometricFillCircle(CanvasDrawingSession ds, Color color, Stroke stroke, StrokeSegment segment)
        {
            ds.FillCircle(stroke.StartingPosition, segment.StartingSize, color);


            float distance = segment.StartingDistance;
            while (distance < segment.Distance)
            {
                float smooth = distance / segment.Distance;
                float smoothR = 1 - smooth;

                float sizePressureSmooth = segment.Size * (smooth * stroke.StartingPressure + smoothR * stroke.Pressure);
                Vector2 positionSmooth = smooth * stroke.StartingPosition + smoothR * stroke.Position;

                distance += segment.Spacing * sizePressureSmooth;


                ds.FillCircle(positionSmooth, sizePressureSmooth, color);
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardness(CanvasDrawingSession ds, byte[] shaderCode, int hardness, Vector4 colorHdr, Stroke stroke, StrokeSegment segment)
        {
            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Properties =
                {
                    ["hardness"] = hardness,
                    ["pressure"] = 1f,
                    ["radius"] = segment.StartingSize,
                    ["targetPosition"] = stroke.StartingPosition,
                    ["color"] = colorHdr
                }
            });


            float distance = segment.StartingDistance;
            while (distance < segment.Distance)
            {
                float smooth = distance / segment.Distance;
                float smoothR = 1 - smooth;

                float sizePressureSmooth = segment.Size * (smooth * stroke.StartingPressure + smoothR * stroke.Pressure);
                Vector2 positionSmooth = smooth * stroke.StartingPosition + smoothR * stroke.Position;

                distance += segment.Spacing * sizePressureSmooth;


                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = 1f,
                        ["radius"] = sizePressureSmooth,
                        ["targetPosition"] = positionSmooth,
                        ["color"] = colorHdr
                    }
                });
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardnessWithTexture(CanvasDrawingSession ds, byte[] shaderCode, int hardness, Vector4 colorHdr, CanvasBitmap texture, bool rotate, Stroke stroke, StrokeSegment segment)
        {
            Vector2 normalization = stroke.Normalize();
            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Source1 = texture,
                Properties =
                {
                    ["hardness"] = hardness,
                    ["rotate"] = rotate,
                    ["normalization"] = normalization,
                    ["pressure"] = 1f,
                    ["radius"] = segment.StartingSize,
                    ["targetPosition"] = stroke.StartingPosition,
                    ["color"] = colorHdr
                }
            });


            float distance = segment.StartingDistance;
            while (distance < segment.Distance)
            {
                float smooth = distance / segment.Distance;
                float smoothR = 1 - smooth;

                float sizePressureSmooth = segment.Size * (smooth * stroke.StartingPressure + smoothR * stroke.Pressure);
                Vector2 positionSmooth = smooth * stroke.StartingPosition + smoothR * stroke.Position;

                distance += segment.Spacing * sizePressureSmooth;


                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = normalization,
                        ["pressure"] = 1f,
                        ["radius"] = sizePressureSmooth,
                        ["targetPosition"] = positionSmooth,
                        ["color"] = colorHdr
                    }
                });
            }
        }

    }
}