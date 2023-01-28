using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Layers.Models
{
    public sealed partial class BitmapLayer
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

    }
}