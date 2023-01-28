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
        public void CapDrawShaderBrushEdgeHardness(StrokeCap cap, byte[] shaderCode, Vector4 colorHdr, float mix = 1, float wet = 12, float persistence = 0, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            this.ConstructMix(cap.StartingPosition);

            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressure = cap.StartingPressure * cap.StartingSize;

                this.Mix(cap.StartingPosition, cap.StartingSize, wet);
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * cap.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? sizePressure : cap.StartingSize,
                        ["targetPosition"] = cap.StartingPosition,
                        ["color"] = this.GetMix(colorHdr, mix)
                    }
                });
            }
        }

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void SegmentDrawShaderBrushEdgeHardness(StrokeSegment segment, byte[] shaderCode, Vector4 colorHdr, float mix = 1, float wet = 12, float persistence = 0, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressure = segment.StartingPressure * segment.Size;

                this.Mix(segment.StartingPosition, sizePressure, wet);
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * segment.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? segment.Size : sizePressure,
                        ["targetPosition"] = segment.StartingPosition,
                        ["color"] = this.GetMix(colorHdr, mix, persistence)
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

                    this.Mix(positionIsometric, sizePressure, wet);
                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["pressure"] = ignoreFlowPressure ? flow : flow * pressureIsometric,
                            ["radius"] = sizePressureIsometric,
                            ["targetPosition"] = positionIsometric,
                            ["color"] = this.GetMix(colorHdr, mix, persistence)
                        }
                    });
                }
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void CapDrawShaderBrushEdgeHardnessWithTexture(StrokeCap cap, byte[] shaderCode, Vector4 colorHdr, CanvasBitmap texture, float mix = 1, float wet = 12, float persistence = 0, bool rotate = false, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            this.ConstructMix(cap.StartingPosition);

            if (rotate) return;

            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressure = cap.StartingPressure * cap.StartingSize;

                this.Mix(cap.StartingPosition, sizePressure, wet);
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = false,
                        ["normalization"] = Vector2.Zero,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * cap.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? sizePressure : cap.StartingSize,
                        ["targetPosition"] = cap.StartingPosition,
                        ["color"] = this.GetMix(colorHdr, mix, persistence)
                    }
                });
            }
        }

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void SegmentDrawShaderBrushEdgeHardnessWithTexture(StrokeSegment segment, byte[] shaderCode, Vector4 colorHdr, CanvasBitmap texture, float mix = 1, float wet = 12, float persistence = 0, bool rotate = false, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressure = segment.StartingPressure * segment.Size;

                this.Mix(segment.StartingPosition, sizePressure, wet);
                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = segment.Normalize,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * segment.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? segment.Size : sizePressure,
                        ["targetPosition"] = segment.StartingPosition,
                        ["color"] = this.GetMix(colorHdr, mix, persistence)
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

                    this.Mix(positionIsometric, sizePressureIsometric, wet);
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
                            ["color"] = this.GetMix(colorHdr, mix, persistence)
                        }
                    });
                }
            }
        }

    }
}