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