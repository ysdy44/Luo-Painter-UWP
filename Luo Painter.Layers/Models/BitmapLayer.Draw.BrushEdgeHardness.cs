using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Layers.Models
{
    partial class BitmapLayer
    {

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void CapDrawShaderBrushEdgeHardness(StrokeCap cap,
            byte[] shaderCode, Vector4 colorHdr,
            int hardness = 0, float flow = 1f,
            int sizePressure = 0, float minSize = 0,
            int flowPressure = 0, float minFlow = 0)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = this.GetPressed(minSize, sizePressure, cap.StartingPressure) * cap.StartingSize;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.GetPressed(minFlow, flowPressure, cap.StartingPressure) * flow,
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
            int sizePressure = 0, float minSize = 0,
            int flowPressure = 0, float minFlow = 0)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = this.GetPressed(minSize, sizePressure, segment.StartingPressure) * segment.Size;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = this.GetPressed(minFlow, flowPressure, segment.StartingPressure) * flow,
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

                    float sizePressureIsometric = this.GetPressed(minSize, sizePressure, pressureIsometric) * segment.Size;
                    distance += segment.Spacing * sizePressureIsometric;

                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["pressure"] = this.GetPressed(minFlow, flowPressure, pressureIsometric) * flow,
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
            int sizePressure = 0, float minSize = 0,
            int flowPressure = 0, float minFlow = 0)
        {
            if (rotate) return;

            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = this.GetPressed(minSize, sizePressure, cap.StartingPressure) * cap.StartingSize;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = false,
                        ["normalization"] = Vector2.Zero,
                        ["pressure"] = this.GetPressed(minFlow, flowPressure, cap.StartingPressure) * flow,
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
            int sizePressure = 0, float minSize = 0,
            int flowPressure = 0, float minFlow = 0)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                float sizePressed = this.GetPressed(minSize, sizePressure, segment.StartingPressure) * segment.Size;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = segment.Normalize,
                        ["pressure"] = this.GetPressed(minFlow, flowPressure, segment.StartingPressure) * flow,
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

                    float sizePressureIsometric = this.GetPressed(minSize, sizePressure, pressureIsometric) * segment.Size;
                    distance += segment.Spacing * sizePressureIsometric;

                    ds.DrawImage(new PixelShaderEffect(shaderCode)
                    {
                        Source1 = texture,
                        Properties =
                        {
                            ["hardness"] = hardness,
                            ["rotate"] = rotate,
                            ["normalization"] = segment.Normalize,
                            ["pressure"] = this.GetPressed(minFlow, flowPressure, pressureIsometric) * flow,
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