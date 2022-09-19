using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Layers.Models
{
    public sealed class AnchorCollection : List<Anchor>, ICacheTransform, IDisposable
    {

        internal readonly CanvasRenderTarget SourceRenderTarget;
        public ICanvasImage Source => this.SourceRenderTarget;
        public Color[] GetPixelColors(int left, int top, int width, int height) => this.SourceRenderTarget.GetPixelColors(left, top, width, height);

        public Color Color { get; set; } = Colors.Black;
        public float StrokeWidth { get; set; } = 1;

        public bool IsClosed { get; set; }
        public Vector2 ClosePoint { get; set; }
        public bool CloseIsSmooth { get; set; }

        public int Index = -1;
        public Anchor SelectedItem => (this.Index is -1) ? null : base[this.Index];


        public AnchorCollection(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            //@DPI
            this.SourceRenderTarget = new CanvasRenderTarget(resourceCreator, width, height, 96);
        }


        public AnchorCollection Clone(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            AnchorCollection anchors = this.CloneCore(resourceCreator, width, height);
            foreach (Anchor item in this)
            {
                anchors.Add(item.Clone());
            }
            anchors.Segment(resourceCreator);
            anchors.Invalidate();
            return anchors;
        }
        public AnchorCollection Clone(ICanvasResourceCreator resourceCreator, int width, int height, Vector2 offset)
        {
            AnchorCollection anchors = this.CloneCore(resourceCreator, width, height);
            foreach (Anchor item in this)
            {
                anchors.Add(item.Clone(offset));
            }
            anchors.Segment(resourceCreator);
            anchors.Invalidate();
            return anchors;
        }
        public AnchorCollection Clone(ICanvasResourceCreator resourceCreator, int width, int height, Matrix3x2 matrix)
        {
            AnchorCollection anchors = this.CloneCore(resourceCreator, width, height);
            foreach (Anchor item in this)
            {
                anchors.Add(item.Clone(matrix));
            }
            anchors.Segment(resourceCreator);
            anchors.Invalidate();
            return anchors;
        }
        private AnchorCollection CloneCore(ICanvasResourceCreator resourceCreator, int width, int height)
        {
            return new AnchorCollection(resourceCreator, width, height)
            {
                Color = this.Color,
                StrokeWidth = this.StrokeWidth,
                IsClosed = this.IsClosed,
                ClosePoint = this.ClosePoint,
                CloseIsSmooth = this.CloseIsSmooth,
                Index = this.Index
            };
        }


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


        public bool Segment(ICanvasResourceCreator resourceCreator)
        {
            // Begin + First + Anchors + Last
            if (this.IsClosed)
            {
                switch (base.Count)
                {
                    case 0:
                        return false;
                    case 1:
                        this.Single().Dispose();
                        return false;
                    case 2:
                        this.First().AddLine(resourceCreator, this.Last(), this.StrokeWidth);
                        this.Last().Dispose();
                        return true;
                    default:
                        break;
                }
            }
            // Begin + First + Anchors + Last + End
            else
            {
                switch (base.Count)
                {
                    case 0:
                        return false;
                    case 1:
                        // Begin + End
                        this.First().AddLine(resourceCreator, this.ClosePoint, this.StrokeWidth);
                        return true;
                    case 2:
                        {
                            Vector2 previousRightControlPoint;


                            // 0. Begin
                            Anchor begin = base[0];
                            Vector2 beginPoint = begin.Point;

                            previousRightControlPoint = beginPoint;


                            // 1. First
                            Anchor first = base[1];
                            Vector2 firstPoint = first.Point;

                            if (first.IsSmooth is false && begin.IsSmooth is false)
                            {
                                begin.AddLine(resourceCreator, first, this.StrokeWidth);
                                previousRightControlPoint = firstPoint;
                            }
                            else
                            {
                                Vector2 leftControlPoint = Anchor.CubicBezierFirst(firstPoint, beginPoint, this.ClosePoint, ref previousRightControlPoint);
                                begin.AddCubicBezier(resourceCreator, beginPoint, leftControlPoint, first, this.StrokeWidth);
                            }


                            // 4. End
                            Anchor last = base[base.Count - 1];

                            if (this.CloseIsSmooth is false && last.IsSmooth is false)
                                first.AddLine(resourceCreator, this.ClosePoint, this.StrokeWidth);
                            else
                                first.AddCubicBezier(resourceCreator, previousRightControlPoint, this.ClosePoint, this.StrokeWidth);
                        }
                        return true;
                    default:
                        break;
                }
            }


            // default
            {
                Vector2 previousRightControlPoint;


                // 0. Begin
                Anchor begin = base[0];
                Vector2 beginPoint = begin.Point;

                previousRightControlPoint = beginPoint;


                // 1. First
                Anchor first = base[1];
                Vector2 firstPoint = first.Point;

                if (first.IsSmooth is false && begin.IsSmooth is false)
                {
                    begin.AddLine(resourceCreator, first, this.StrokeWidth);
                    previousRightControlPoint = firstPoint;
                }
                else
                {
                    Anchor next = base[2];
                    Vector2 nextPoint = next.Point;

                    Vector2 leftControlPoint = Anchor.CubicBezierFirst(firstPoint, beginPoint, nextPoint, ref previousRightControlPoint);
                    begin.AddCubicBezier(resourceCreator, beginPoint, leftControlPoint, first, this.StrokeWidth);
                }


                // 2. Anchors
                for (int i = 2; i < base.Count - 1; i++)
                {
                    Anchor current = base[i];
                    Anchor previous = base[i - 1];
                    Vector2 point = current.Point;
                    Vector2 previousPoint = previous.Point;

                    if (current.IsSmooth is false && previous.IsSmooth is false)
                    {
                        previous.AddLine(resourceCreator, current, this.StrokeWidth);
                        previousRightControlPoint = point;
                    }
                    else
                    {
                        Anchor next = base[i + 1];
                        Vector2 nextPoint = next.Point;

                        Vector2 rightControlPoint = previousRightControlPoint;
                        Vector2 leftControlPoint = Anchor.CubicBezier(point, previousPoint, nextPoint, ref previousRightControlPoint);
                        previous.AddCubicBezier(resourceCreator, rightControlPoint, leftControlPoint, current, this.StrokeWidth);
                    }
                }


                if (this.IsClosed is false)
                {
                    // 3. Last
                    Anchor current = base[base.Count - 1];
                    Anchor previous = base[base.Count - 2];
                    Vector2 point = current.Point;
                    Vector2 previousPoint = previous.Point;

                    Vector2 rightControlPoint = previousRightControlPoint;
                    Vector2 leftControlPoint = Anchor.CubicBezier(point, previousPoint, this.ClosePoint, ref previousRightControlPoint);

                    if (current.IsSmooth is false && previous.IsSmooth is false)
                        previous.AddLine(resourceCreator, current, this.StrokeWidth);
                    else
                        previous.AddCubicBezier(resourceCreator, rightControlPoint, leftControlPoint, current, this.StrokeWidth);


                    // 4. End
                    if (this.CloseIsSmooth is false && current.IsSmooth is false)
                        current.AddLine(resourceCreator, this.ClosePoint, this.StrokeWidth);
                    else
                        current.AddCubicBezier(resourceCreator, previousRightControlPoint, this.ClosePoint, this.StrokeWidth);
                }
                else
                {
                    // 3. Last
                    Anchor current = base[base.Count - 1];
                    Anchor previous = base[base.Count - 2];

                    if (current.IsSmooth is false && previous.IsSmooth is false)
                        previous.AddLine(resourceCreator, current, this.StrokeWidth);
                    else
                        previous.AddCubicBezier(resourceCreator, previousRightControlPoint, current, this.StrokeWidth);

                    current.Dispose();
                }
            }
            return true;
        }


        /// <summary>
        /// Cache the AnchorCollection's transformer.
        /// </summary>
        public void CacheTransform()
        {
            foreach (Anchor item in this)
            {
                item.CacheTransform();
            }
        }
        /// <summary>
        /// Cache the AnchorCollection's transformer.
        /// </summary>
        public void CacheTransformOnlySelected()
        {
            foreach (Anchor item in this)
            {
                if (item.IsChecked)
                {
                    item.CacheTransform();
                }
            }
        }

        /// <summary>
        /// Transforms the anchor by the given vector.
        /// </summary>
        /// <param name="vector"> The add value use to summed. </param>
        public void TransformAdd(Vector2 vector)
        {
            foreach (Anchor item in this)
            {
                item.TransformAdd(vector);
            }
        }
        /// <summary>
        /// Transforms the anchor by the given vector.
        /// </summary>
        /// <param name="vector"> The add value use to summed. </param>
        public void TransformAddOnlySelected(Vector2 vector)
        {
            foreach (Anchor item in this)
            {
                if (item.IsChecked)
                {
                    item.TransformAdd(vector);
                }
            }
        }

        /// <summary>
        /// Transforms the anchor by the given matrix.
        /// </summary>
        /// <param name="matrix"> The resulting matrix. </param>
        public void TransformMultiplies(Matrix3x2 matrix)
        {
            foreach (Anchor item in this)
            {
                item.TransformMultiplies(matrix);
            }
        }
        /// <summary>
        /// Transforms the anchor by the given matrix.
        /// </summary>
        /// <param name="matrix"> The resulting matrix. </param>  
        public void TransformMultipliesOnlySelected(Matrix3x2 matrix)
        {
            foreach (Anchor item in this)
            {
                if (item.IsChecked)
                {
                    item.TransformMultiplies(matrix);
                }
            }
        }


        /// <summary>
        /// Check anchor which in the rect.
        /// </summary>
        /// <param name="left"> The destination rectangle's left. </param>
        /// <param name="top"> The destination rectangle's top. </param>
        /// <param name="right"> The destination rectangle's right. </param>
        /// <param name="bottom"> The destination rectangle's bottom. </param>
        public void RectChoose(float left, float top, float right, float bottom)
        {
            foreach (Anchor item in this)
            {
                bool isContained = item.Contained(left, top, right, bottom);
                if (item.IsChecked == isContained) continue;
                item.IsChecked = isContained;
            }
        }
        /// <summary>
        /// Check anchor which in the rect.
        /// </summary>
        /// <param name="boxRect"> The destination rectangle. </param>
        public void BoxChoose(TransformerRect boxRect) => this.RectChoose(boxRect.Left, boxRect.Top, boxRect.Right, boxRect.Bottom);


        public void Dispose()
        {
            this.Source.Dispose();
            foreach (Anchor item in this)
            {
                item.Dispose();
            }
        }

    }
}