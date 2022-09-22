using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed partial class AnchorCollection : List<Anchor>, ICacheTransform, IDisposable
    {
        public void Invalidate()
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
                foreach (Anchor anchor in this)
                {
                    foreach (Vector3 item in anchor.Strokes)
                    {
                        ds.FillCircle(item.X, item.Y, item.Z, this.Color);
                    }
                }
            }
        }

        /// <summary>
        /// Begin + First + Anchors + Last + End
        /// </summary>
        public bool Segment(ICanvasResourceCreator resourceCreator)
        {
            switch (this.IsClosed ? base.Count : this.Count + 1)
            {
                case 0:
                case 1:
                    return false;
                case 2:
                    // Begin
                    this.First().AddLine(resourceCreator, this.IsClosed ? this.Last().Point : this.ClosePoint, this.StrokeWidth);
                    return true;
                case 3:
                    {
                        Anchor begin = base[0];
                        Anchor first = base[1];
                        Vector2 firstPoint = first.Point;

                        // Begin
                        if (first.IsSmooth is false && begin.IsSmooth is false)
                        {
                            first.LeftControlPoint = firstPoint;
                            first.RightControlPoint = firstPoint;
                            begin.AddLine(resourceCreator, first, this.StrokeWidth);
                        }
                        else
                        {
                            Vector2 vector = ClosePoint - (firstPoint + begin.Point) / 2;

                            float left = (firstPoint - (firstPoint + begin.Point) / 2).Length();
                            float right = (firstPoint - ClosePoint).Length();
                            float length = left + right;

                            first.LeftControlPoint = firstPoint - System.Math.Min(left, right) / length * vector;
                            first.RightControlPoint = firstPoint + right / length / 2 * vector;
                            begin.AddCubicBezier(resourceCreator, begin.Point, first.LeftControlPoint, first, this.StrokeWidth);
                        }

                        // End
                        if (this.IsClosed)
                        {
                            Anchor end = base[2];
                            Vector2 endPoint = end.Point;

                            end.LeftControlPoint = endPoint;
                            end.RightControlPoint = endPoint;

                            if (end.IsSmooth is false && first.IsSmooth is false)
                                first.AddLine(resourceCreator, endPoint, this.StrokeWidth);
                            else
                                first.AddCubicBezier(resourceCreator, first.RightControlPoint, endPoint, endPoint, this.StrokeWidth);
                        }
                        else
                        {
                            if (this.CloseIsSmooth is false && first.IsSmooth is false)
                                first.AddLine(resourceCreator, this.ClosePoint, this.StrokeWidth);
                            else
                                first.AddCubicBezier(resourceCreator, first.RightControlPoint, this.ClosePoint, this.ClosePoint, this.StrokeWidth);
                        }
                    }
                    return true;
                default:
                    {
                        Anchor begin = base[0];
                        Anchor first = base[1];
                        Vector2 firstPoint = first.Point;

                        // Begin
                        if (first.IsSmooth is false && begin.IsSmooth is false)
                        {
                            first.LeftControlPoint = firstPoint;
                            first.RightControlPoint = firstPoint;
                            begin.AddLine(resourceCreator, firstPoint, this.StrokeWidth);
                        }
                        else
                        {
                            Anchor next = base[2];
                            Vector2 beginPoint = begin.Point;

                            Vector2 vector = next.LeftControlPoint - (firstPoint + beginPoint) / 2;

                            float left = (firstPoint - (firstPoint + beginPoint) / 2).LengthSquared();
                            float right = (firstPoint - next.Point).LengthSquared();
                            float length = left + right;

                            first.LeftControlPoint = firstPoint - left / length * vector;
                            first.RightControlPoint = firstPoint + right / length / 2 * vector;
                            begin.AddCubicBezier(resourceCreator, beginPoint, first.LeftControlPoint, firstPoint, this.StrokeWidth);
                        }

                        // First + Anchors
                        for (int i = 2; i < base.Count - 1; i++)
                        {
                            Anchor current = base[i];
                            Anchor previous = base[i - 1];
                            Vector2 point = current.Point;

                            if (current.IsSmooth is false && previous.IsSmooth is false)
                            {
                                current.LeftControlPoint = point;
                                current.RightControlPoint = point;
                                previous.AddLine(resourceCreator, point, this.StrokeWidth);
                            }
                            else
                            {
                                Anchor next = base[i + 1];

                                Vector2 vector = next.LeftControlPoint - previous.RightControlPoint;

                                float left = (point - previous.Point).LengthSquared();
                                float right = (point - next.Point).LengthSquared();
                                float length = left + right;

                                current.LeftControlPoint = point - left / length / 2 * vector;
                                current.RightControlPoint = point + right / length / 2 * vector;
                                previous.AddCubicBezier(resourceCreator, previous.RightControlPoint, current.LeftControlPoint, point, this.StrokeWidth);
                            }
                        }

                        // Last + End
                        if (this.IsClosed is false)
                        {
                            Anchor last = base[base.Count - 2];
                            Anchor end = base[base.Count - 1];
                            Vector2 endPoint = end.Point;

                            if (end.IsSmooth is false && last.IsSmooth is false)
                            {
                                end.LeftControlPoint = endPoint;
                                end.RightControlPoint = endPoint;
                                last.AddLine(resourceCreator, endPoint, this.StrokeWidth);
                            }
                            else
                            {
                                Vector2 vector = this.ClosePoint - last.RightControlPoint;

                                float left = (endPoint - last.Point).LengthSquared();
                                float right = (endPoint - this.ClosePoint).LengthSquared();
                                float length = left + right;

                                end.LeftControlPoint = endPoint - left / length / 2 * vector;
                                end.RightControlPoint = endPoint + right / length / 2 * vector;
                                last.AddCubicBezier(resourceCreator, last.RightControlPoint, end.LeftControlPoint, endPoint, this.StrokeWidth);
                            }

                            if (this.CloseIsSmooth is false && end.IsSmooth is false)
                                end.AddLine(resourceCreator, this.ClosePoint, this.StrokeWidth);
                            else
                                end.AddCubicBezier(resourceCreator, end.RightControlPoint, this.ClosePoint, this.ClosePoint, this.StrokeWidth);
                        }
                    }
                    return true;
            }
        }
    }
}