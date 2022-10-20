using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public partial struct StrokeSegment
    {
        //@Static
        public static readonly CanvasStrokeStyle CanvasStrokeStyle = new CanvasStrokeStyle
        {
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

        public void DrawLine(CanvasDrawingSession ds, Color color, bool ignoreSizePressure = false)
        {
            ds.DrawLine(this.StartingPosition, this.Position, color, ignoreSizePressure ? this.Size : this.StartingSize * 2, StrokeSegment.CanvasStrokeStyle);
        }

        public void IsometricShape(CanvasDrawingSession ds, Color color, PenTipShape shape = PenTipShape.Circle, bool isStroke = false, bool ignoreSizePressure = false)
        {
            switch (shape)
            {
                case PenTipShape.Circle:
                    {
                        if (isStroke)
                            ds.DrawCircle(this.StartingPosition, ignoreSizePressure ? this.Size : this.StartingSize, color);
                        else
                            ds.FillCircle(this.StartingPosition, ignoreSizePressure ? this.Size : this.StartingSize, color);

                        float distance = this.StartingDistance;
                        while (distance < this.Distance)
                        {
                            float smooth = distance / this.Distance;

                            float pressureIsometric = this.Pressure * (1 - smooth) + this.StartingPressure * smooth;
                            Vector2 positionIsometric = Vector2.Lerp(this.Position, this.StartingPosition, smooth);

                            float sizePressureIsometric = ignoreSizePressure ? this.Size : (this.Size * pressureIsometric);
                            distance += this.Spacing * sizePressureIsometric;

                            if (isStroke)
                                ds.DrawCircle(positionIsometric, sizePressureIsometric, color);
                            else
                                ds.FillCircle(positionIsometric, sizePressureIsometric, color);
                        }
                    }
                    break;
                case PenTipShape.Rectangle:
                    {
                        float sizePressure = ignoreSizePressure ? this.Size : this.StartingSize;
                        if (isStroke)
                            ds.DrawRectangle(this.StartingPosition.X - sizePressure, this.StartingPosition.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);
                        else
                            ds.FillRectangle(this.StartingPosition.X - sizePressure, this.StartingPosition.Y - sizePressure, sizePressure + sizePressure, sizePressure + sizePressure, color);

                        float distance = this.StartingDistance;
                        while (distance < this.Distance)
                        {
                            float smooth = distance / this.Distance;

                            float pressureIsometric = this.Pressure * (1 - smooth) + this.StartingPressure * smooth;
                            Vector2 positionIsometric = Vector2.Lerp(this.Position, this.StartingPosition, smooth);

                            float sizePressureIsometric = ignoreSizePressure ? this.Size : (this.Size * pressureIsometric);
                            distance += this.Spacing * sizePressureIsometric;

                            if (isStroke)
                                ds.DrawRectangle(positionIsometric.X - sizePressureIsometric, positionIsometric.Y - sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, color);
                            else
                                ds.FillRectangle(positionIsometric.X - sizePressureIsometric, positionIsometric.Y - sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, sizePressureIsometric + sizePressureIsometric, color);
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
        public void IsometricDrawShaderBrushEdgeHardness(CanvasDrawingSession ds, byte[] shaderCode, Vector4 colorHdr, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Properties =
                {
                    ["hardness"] = hardness,
                    ["pressure"] = ignoreFlowPressure ? flow : flow * this.StartingPressure,
                    ["radius"] =  ignoreSizePressure ? this.Size : this.StartingSize,
                    ["targetPosition"] = this.StartingPosition,
                    ["color"] = colorHdr
                }
            });

            float distance = this.StartingDistance;
            while (distance < this.Distance)
            {
                float smooth = distance / this.Distance;

                float pressureIsometric = this.Pressure * (1 - smooth) + this.StartingPressure * smooth;
                Vector2 positionIsometric = Vector2.Lerp(this.Position, this.StartingPosition, smooth);

                float sizePressureIsometric = ignoreSizePressure ? this.Size : (this.Size * pressureIsometric);
                distance += this.Spacing * sizePressureIsometric;

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


        /// <summary>
        /// <see cref="ShaderType.BrushEdgeHardnessWithTexture"/>
        /// </summary>
        public void IsometricDrawShaderBrushEdgeHardnessWithTexture(CanvasDrawingSession ds, byte[] shaderCode, Vector4 colorHdr, CanvasBitmap texture, bool rotate = false, int hardness = 0, float flow = 1f, bool ignoreSizePressure = false, bool ignoreFlowPressure = false)
        {
            ds.DrawImage(new PixelShaderEffect(shaderCode)
            {
                Source1 = texture,
                Properties =
                {
                    ["hardness"] = hardness,
                    ["rotate"] = rotate,
                    ["normalization"] = this.Normalize,
                    ["pressure"] = ignoreFlowPressure ? flow : flow * this.StartingPressure,
                    ["radius"] =  ignoreSizePressure ? this.Size : this.StartingSize,
                    ["targetPosition"] = this.StartingPosition,
                    ["color"] = colorHdr
                }
            });

            float distance = this.StartingDistance;
            while (distance < this.Distance)
            {
                float smooth = distance / this.Distance;

                float pressureIsometric = this.Pressure * (1 - smooth) + this.StartingPressure * smooth;
                Vector2 positionIsometric = Vector2.Lerp(this.Position, this.StartingPosition, smooth);

                float sizePressureIsometric = ignoreSizePressure ? this.Size : (this.Size * pressureIsometric);
                distance += this.Spacing * sizePressureIsometric;

                ds.DrawImage(new PixelShaderEffect(shaderCode)
                {
                    Source1 = texture,
                    Properties =
                    {
                        ["hardness"] = hardness,
                        ["rotate"] = rotate,
                        ["normalization"] = this.Normalize,
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