using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers
{
    public sealed class BSpline : List<BSplinePoints>
    {
        public bool IsClosed { get; set; } 

        Vector2 Point;

        private BSplineType GetType(int i)
        {
            if (i is 0) return BSplineType.BeginFigure;
            else if (i < base.Count - 2) return BSplineType.Node;
            else if (i < base.Count - 1) return BSplineType.EndFigure;
            else if (this.IsClosed) return BSplineType.Close;
            else return BSplineType.None;
        }

        public void Arrange()
        {
            switch (base.Count)
            {
                case 0:
                case 1:
                case 2:
                    foreach (BSplinePoints item in this)
                    {
                        item.Clear();
                    }
                    break;
                default:
                    this.Point = default;

                    for (int i = 0; i < base.Count; i++)
                    {
                        switch (this.GetType(i))
                        {
                            case BSplineType.BeginFigure:
                                base[i].Spline
                                (
                                    base[this.IsClosed ? base.Count - 1 : i].Point,
                                    base[i + 1].Point,
                                    base[i + 2].Point
                                );
                                break;
                            case BSplineType.Node:
                                base[i].Spline
                                (
                                    base[i - 1].Point,
                                    base[i + 1].Point,
                                    base[i + 2].Point
                                );
                                break;
                            case BSplineType.EndFigure:
                                base[i].Spline
                                (
                                    base[i - 1].Point,
                                    base[i + 1].Point,
                                    base[this.IsClosed ? 0 : i + 1].Point
                                );
                                break;
                            case BSplineType.Close:
                                base[i].Spline
                                (
                                    base[i - 1].Point,
                                    base[0].Point,
                                    base[1].Point
                                );
                                break;
                            default: continue;
                        }
                    }
                    break;
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            switch (base.Count)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    ds.DrawLine(base[0].Point, base[1].Point, Colors.DodgerBlue);
                    break;
                default:
                    foreach (BSplinePoints item in this)
                    {
                        foreach (Vector2 p in item)
                        {
                            if (this.Point != default) ds.DrawLine(this.Point, p, Colors.DodgerBlue);
                            this.Point = p;
                        }
                    }
                    break;
            }
        }
    }
}