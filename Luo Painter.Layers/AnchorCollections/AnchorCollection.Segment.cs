using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed partial class AnchorCollection
    {
        /// <summary>
        /// Called when the value of properties that describe the size and location of the <see cref="Anchor.Strokes"/> change.
        /// </summary>
        public void Invalidate()
        {
            using (CanvasDrawingSession ds = this.SourceRenderTarget.CreateDrawingSession())
            {
                //@DPI 
                ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">

                ds.Clear(Colors.Transparent);
                foreach (Anchor anchor in this)
                {
                    ds.FillCircle(anchor.Point, anchor.Pressure * this.StrokeWidth / 2, this.Color);

                    foreach (Vector3 item in anchor.Strokes)
                    {
                        ds.FillCircle(item.X, item.Y, item.Z, this.Color);
                    }
                }

                // End~ (ClosePoint)
                if (this.IsClosed is false) ds.FillCircle(this.ClosePoint, this.StrokeWidth / 2, this.Color);
            }
        }

        /// <summary>
        /// Causes the <see cref="AnchorCollection"/> to load a new Strokes into the <see cref="Anchor"/> using the specified <see cref="AnchorCollection.ClosePoint"/>.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <param name="updateAll">
        /// **True** to update all when changing the segments; otherwise, **False** to update Last and End. The default is **True**.
        /// </param>
        /// <returns> **True** if the segments is changed; otherwise, **False**. </returns>
        public bool Segment(ICanvasResourceCreator resourceCreator, bool updateAll = true)
        {
            // IsClosed is true
            // Begin~ First~ Anchors~ Last~

            // IsClosed is false
            // Begin~ First~ Anchors~ Last~ End~ (ClosePoint)

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

                        // Begin
                        if (first.IsSmooth is false && begin.IsSmooth is false)
                        {
                            first.SegmentLine();
                            begin.AddLine(resourceCreator, first, this.StrokeWidth);
                        }
                        else
                        {
                            first.SegmentFirst(this.ClosePoint, begin.Point);
                            begin.AddCubicBezier(resourceCreator, begin.Point, first.LeftControlPoint, first, this.StrokeWidth);
                        }

                        // End
                        if (this.IsClosed)
                        {
                            Anchor end = base[2];

                            if (end.IsSmooth is false && first.IsSmooth is false)
                                first.AddLine(resourceCreator, end.Point, this.StrokeWidth);
                            else
                                first.AddCubicBezier(resourceCreator, first.RightControlPoint, end.Point, end, this.StrokeWidth);

                            end.SegmentLine();
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
                        if (this.IsClosed || updateAll)
                        {
                            Anchor begin = base[0];
                            Anchor first = base[1];

                            // Begin
                            if (first.IsSmooth is false && begin.IsSmooth is false)
                            {
                                first.SegmentLine();
                                begin.AddLine(resourceCreator, first, this.StrokeWidth);
                            }
                            else
                            {
                                Anchor next = base[2];

                                first.SegmentFirst(next.Point, begin.Point);
                                begin.AddCubicBezier(resourceCreator, begin.Point, first.LeftControlPoint, first, this.StrokeWidth);
                            }

                            // First + Anchors
                            for (int i = 2; i < base.Count - 1; i++)
                            {
                                Anchor current = base[i];
                                Anchor previous = base[i - 1];

                                if (current.IsSmooth is false && previous.IsSmooth is false)
                                {
                                    current.SegmentLine();
                                    previous.AddLine(resourceCreator, current, this.StrokeWidth);
                                }
                                else
                                {
                                    Anchor next = base[i + 1];

                                    current.Segment(next.Point, previous.Point);
                                    previous.AddCubicBezier(resourceCreator, previous.RightControlPoint, current.LeftControlPoint, current, this.StrokeWidth);
                                }
                            }
                        }

                        // Last
                        Anchor last = base[base.Count - 2];
                        Anchor end = base[base.Count - 1];

                        if (end.IsSmooth is false && last.IsSmooth is false)
                        {
                            end.SegmentLine();
                            last.AddLine(resourceCreator, end, this.StrokeWidth);
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