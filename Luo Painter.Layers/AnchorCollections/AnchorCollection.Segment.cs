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
                    if (this.IsClosed)
                    {
                        this.First().AddLine(resourceCreator, this.Last(), this.StrokeWidth);
                        this.Last().Dispose();
                    }
                    else
                    {
                        this.First().AddLine(resourceCreator, this.ClosePoint, this.StrokeWidth);
                    }
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
                            Vector2 beginPoint = begin.Point;

                            first.Segment(this.ClosePoint, (firstPoint + beginPoint) / 2);
                            begin.AddCubicBezier(resourceCreator, beginPoint, first.LeftControlPoint, first, this.StrokeWidth);
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
                                first.AddCubicBezier(resourceCreator, first.RightControlPoint, endPoint, end, this.StrokeWidth);

                            end.Dispose();
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

                            first.Segment(next.Point, (firstPoint + beginPoint) / 2);
                            begin.AddCubicBezier(resourceCreator, beginPoint, first.LeftControlPoint, first, this.StrokeWidth);
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

                                current.Segment(next.Point, previous.Point);
                                previous.AddCubicBezier(resourceCreator, previous.RightControlPoint, current.LeftControlPoint, current, this.StrokeWidth);
                            }
                        }

                        // Last
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
                            end.Segment(this.ClosePoint, last.Point);
                            last.AddCubicBezier(resourceCreator, last.RightControlPoint, end.LeftControlPoint, end, this.StrokeWidth);
                        }

                        //  End
                        if (this.IsClosed)
                        {
                            end.Dispose();
                        }
                        else
                        {
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