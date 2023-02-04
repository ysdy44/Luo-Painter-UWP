using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer
    {

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void CapDrawShaderBrushEdgeHardness(StrokeCap cap, 
            byte[] shaderCode, Vector4 colorHdr,
            int hardness = 0, float flow = 1f, 
            bool ignoreSizePressure = false,
            bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = ignoreSizePressure ? cap.StartingSize : cap.StartingPressure * cap.StartingSize;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * cap.StartingPressure,
                        ["radius"] = sizePressed,
                        ["targetPosition"] = cap.StartingPosition,
                        ["color"] = colorHdr
                    }
                });
            }
        }

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void SegmentDrawShaderBrushEdgeHardness(StrokeSegment segment, 
            byte[] shaderCode, Vector4 colorHdr, 
            int hardness = 0, float flow = 1f, 
            bool ignoreSizePressure = false, 
            bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = ignoreSizePressure ? segment.Size : segment.StartingPressure * segment.Size;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * segment.StartingPressure,
                        ["radius"] = sizePressed,
                        ["targetPosition"] = segment.StartingPosition,
                        ["color"] = colorHdr
                    }
                });

                float distance = segment.Radius;
                while (distance < segment.Distance)
                {
                    float smooth = distance / segment.Distance;

                    float pressureIsometric = segment.Pressure * (1 - smooth) + segment.StartingPressure * smooth;
                    Vector2 positionIsometric = Vector2.Lerp(segment.Position, segment.StartingPosition, smooth);

                    float sizePressureIsometric = ignoreSizePressure ? segment.Size : (segment.Size * pressureIsometric);
                    distance += segment.Spacing * sizePressureIsometric;

                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["pressure"] = ignoreFlowPressure ? flow : flow * pressureIsometric,
                            ["radius"] = sizePressureIsometric,
                            ["targetPosition"] = positionIsometric,
                            ["color"] = colorHdr
                        }
                    });
                }
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void CapDrawShaderBrushEdgeHardnessWithTexture(StrokeCap cap, 
            byte[] shaderCode, Vector4 colorHdr, 
            CanvasBitmap texture, bool rotate = false, 
            int hardness = 0, float flow = 1f, 
            bool ignoreSizePressure = false, 
            bool ignoreFlowPressure = false)
        {
            if (rotate) return;

            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = ignoreSizePressure ? cap.StartingSize : cap.StartingPressure * cap.StartingSize;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = false,
                        ["normalization"] = Vector2.Zero,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * cap.StartingPressure,
                        ["radius"] = sizePressed,
                        ["targetPosition"] = cap.StartingPosition,
                        ["color"] = colorHdr
                    }
                });
            }
        }

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void SegmentDrawShaderBrushEdgeHardnessWithTexture(StrokeSegment segment, 
            byte[] shaderCode, Vector4 colorHdr, 
            CanvasBitmap texture, bool rotate = false,
            int hardness = 0, float flow = 1f, 
            bool ignoreSizePressure = false, 
            bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = ignoreSizePressure ? segment.Size : segment.StartingPressure * segment.Size;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = segment.Normalize,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * segment.StartingPressure,
                        ["radius"] = sizePressed,
                        ["targetPosition"] = segment.StartingPosition,
                        ["color"] = colorHdr
                    }
                });

                float distance = segment.Radius;
                while (distance < segment.Distance)
                {
                    float smooth = distance / segment.Distance;

                    float pressureIsometric = segment.Pressure * (1 - smooth) + segment.StartingPressure * smooth;
                    Vector2 positionIsometric = Vector2.Lerp(segment.Position, segment.StartingPosition, smooth);

                    float sizePressureIsometric = ignoreSizePressure ? segment.Size : (segment.Size * pressureIsometric);
                    distance += segment.Spacing * sizePressureIsometric;

                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Source1 = texture,
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["rotate"] = rotate,
                            ["normalization"] = segment.Normalize,
                            ["pressure"] = ignoreFlowPressure ? flow : flow * pressureIsometric,
                            ["radius"] = sizePressureIsometric,
                            ["targetPosition"] = positionIsometric,
                            ["color"] = colorHdr
                        }
                    });
                }
            }
        }

    }
}