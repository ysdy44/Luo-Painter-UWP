using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer : LayerBase, ILayer
    {

        public void DrawLine(StrokeSegment segment, Color color, bool ignoreSizePressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawLine(segment.StartingPosition, segment.Position, color, ignoreSizePressure ? segment.Size : segment.StartingSize * 2, BitmapLayer.CanvasStrokeStyle);
            }
        }


        public void CapTip(StrokeCap cap, Color color, PenTipShape shape = PenTipShape.Circle, bool isStroke = false, bool ignoreSizePressure = false)
        {
            switch (shape)
            {
                case PenTipShape.Circle:
                    {
                        using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                        {
                            //@DPI 
                            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                            if (isStroke)
                                ds.DrawCircle(cap.StartingPosition, ignoreSizePressure ? cap.Size : cap.StartingSize, color);
                            else
                                ds.FillCircle(cap.StartingPosition, ignoreSizePressure ? cap.Size : cap.StartingSize, color);
                        }
                    }
                    break;
                case PenTipShape.Rectangle:
                    {
                        using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                        {
                            //@DPI 
                            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                            float sizePressure = ignoreSizePressure ? cap.Size : cap.StartingSize;
                            if (isStroke)
                                ds.DrawRectangle(cap.StartingPosition.X - sizePressure, cap.StartingPosition.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);
                            else
                                ds.FillRectangle(cap.StartingPosition.X - sizePressure, cap.StartingPosition.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void SegmentTip(StrokeSegment segment, Color color, PenTipShape shape = PenTipShape.Circle, bool isStroke = false, bool ignoreSizePressure = false)
        {
            switch (shape)
            {
                case PenTipShape.Circle:
                    {
                        using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                        {
                            //@DPI 
                            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                            if (isStroke)
                                ds.DrawCircle(segment.StartingPosition, ignoreSizePressure ? segment.Size : segment.StartingSize, color);
                            else
                                ds.FillCircle(segment.StartingPosition, ignoreSizePressure ? segment.Size : segment.StartingSize, color);

                            float distance = segment.StartingDistance;
                            while (distance < segment.Distance)
                            {
                                float smooth = distance / segment.Distance;

                                float pressureIsometric = segment.Pressure * (1 - smooth) + segment.StartingPressure * smooth;
                                Vector2 positionIsometric = Vector2.Lerp(segment.Position, segment.StartingPosition, smooth);

                                float sizePressureIsometric = ignoreSizePressure ? segment.Size : (segment.Size * pressureIsometric);
                                distance += segment.Spacing * sizePressureIsometric;

                                if (isStroke)
                                    ds.DrawCircle(positionIsometric, sizePressureIsometric, color);
                                else
                                    ds.FillCircle(positionIsometric, sizePressureIsometric, color);
                            }
                        }
                    }
                    break;
                case PenTipShape.Rectangle:
                    {
                        using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                        {
                            //@DPI 
                            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                            float sizePressure = ignoreSizePressure ? segment.Size : segment.StartingSize;
                            if (isStroke)
                                ds.DrawRectangle(segment.StartingPosition.X - sizePressure, segment.StartingPosition.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);
                            else
                                ds.FillRectangle(segment.StartingPosition.X - sizePressure, segment.StartingPosition.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);

                            float distance = segment.StartingDistance;
                            while (distance < segment.Distance)
                            {
                                float smooth = distance / segment.Distance;

                                float pressureIsometric = segment.Pressure * (1 - smooth) + segment.StartingPressure * smooth;
                                Vector2 positionIsometric = Vector2.Lerp(segment.Position, segment.StartingPosition, smooth);

                                float sizePressureIsometric = ignoreSizePressure ? segment.Size : (segment.Size * pressureIsometric);
                                distance += segment.Spacing * sizePressureIsometric;

                                if (isStroke)
                                    ds.DrawRectangle(positionIsometric.X - sizePressureIsometric, positionIsometric.Y - sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, color);
                                else
                                    ds.FillRectangle(positionIsometric.X - sizePressureIsometric, positionIsometric.Y - sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, color);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void CapDrawShaderBrushEdgeHardness(StrokeCap cap, byte[] shaderCode, Vector4 colorHdr, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, cap.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * cap.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? cap.Size : cap.StartingSize,
                        ["targetPosition"] = cap.StartingPosition,
                        ["color"] = colorHdr
                    }
                });
            }
        }

        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardness"/>
        /// </summary>
        public void SegmentDrawShaderBrushEdgeHardness(StrokeSegment segment, byte[] shaderCode, Vector4 colorHdr, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * segment.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? segment.Size : segment.StartingSize,
                        ["targetPosition"] = segment.StartingPosition,
                        ["color"] = colorHdr
                    }
                });

                float distance = segment.StartingDistance;
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
        public void SegmentDrawShaderBrushEdgeHardnessWithTexture(StrokeSegment segment, byte[] shaderCode, Vector4 colorHdr, CanvasBitmap texture, bool rotate = false, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
            using (ds.CreateLayer(1f, segment.Bounds))
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = segment.Normalize,
                        ["pressure"] = ignoreFlowPressure ? flow : flow * segment.StartingPressure,
                        ["radius"] =  ignoreSizePressure ? segment.Size : segment.StartingSize,
                        ["targetPosition"] = segment.StartingPosition,
                        ["color"] = colorHdr
                    }
                });

                float distance = segment.StartingDistance;
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