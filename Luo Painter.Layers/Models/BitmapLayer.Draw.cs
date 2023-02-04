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

                float sizePressed = ignoreSizePressure ? segment.Size : segment.StartingPressure * segment.Size;

                ds.DrawLine(segment.StartingPosition, segment.Position, color, sizePressed + sizePressed, BitmapLayer.CanvasStrokeStyle);
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

                            float sizePressed = ignoreSizePressure ? cap.StartingSize : cap.StartingPressure * cap.StartingSize;
                            
                            if (isStroke)
                                ds.DrawCircle(cap.StartingPosition, sizePressed, color);
                            else
                                ds.FillCircle(cap.StartingPosition, sizePressed, color);
                        }
                    }
                    break;
                case PenTipShape.Rectangle:
                    {
                        using (CanvasDrawingSession ds = this.TempRenderTarget.CreateDrawingSession())
                        {
                            //@DPI 
                            ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                            float sizePressed = ignoreSizePressure ? cap.StartingSize : cap.StartingPressure * cap.StartingSize;

                            if (isStroke)
                                ds.DrawRectangle(cap.StartingPosition.X - sizePressed, cap.StartingPosition.Y - sizePressed, sizePressed + sizePressed, sizePressed + sizePressed, color);
                            else
                                ds.FillRectangle(cap.StartingPosition.X - sizePressed, cap.StartingPosition.Y - sizePressed, sizePressed + sizePressed, sizePressed + sizePressed, color);
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

                            float sizePressed = ignoreSizePressure ? segment.Size : segment.StartingPressure * segment.Size;

                            if (isStroke)
                                ds.DrawCircle(segment.StartingPosition, sizePressed, color);
                            else
                                ds.FillCircle(segment.StartingPosition, sizePressed, color);

                            float distance = segment.Radius;
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

                            float sizePressed = ignoreSizePressure ? segment.Size : segment.StartingPressure * segment.Size;

                            if (isStroke)
                                ds.DrawRectangle(segment.StartingPosition.X - sizePressed, segment.StartingPosition.Y - sizePressed, sizePressed + sizePressed, sizePressed + sizePressed, color);
                            else
                                ds.FillRectangle(segment.StartingPosition.X - sizePressed, segment.StartingPosition.Y - sizePressed, sizePressed + sizePressed, sizePressed + sizePressed, color);

                            float distance = segment.Radius;
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