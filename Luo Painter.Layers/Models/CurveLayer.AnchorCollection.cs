using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Collections.Generic;
using System.Numerics;

namespace Luo_Painter.Layers.Models
{
    public sealed class AnchorCollection : List<Anchor>
    {

        public int Index = -1;
        public Anchor SelectedItem => (this.Index is -1) ? null : base[this.Index];
        public CanvasGeometry Geometry { get; private set; }


        /// <summary>
        /// (Begin + First + Anchors + Last).
        /// </summary> 
        public void BuildGeometry(ICanvasResourceCreator resourceCreator)
        {
            this.Geometry?.Dispose();

            switch (base.Count)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    {
                        // Counterclockwise
                        CanvasPathBuilder pathBuilder = new CanvasPathBuilder(resourceCreator);

                        // 0. Begin
                        Vector2 beginPoint = base[0].Point;
                        pathBuilder.BeginFigure(beginPoint, default);

                        // 3. Last
                        Vector2 lastPoint = base[1].Point;
                        pathBuilder.AddLine(lastPoint);

                        pathBuilder.EndFigure(CanvasFigureLoop.Open);
                        this.Geometry = CanvasGeometry.CreatePath(pathBuilder);
                        break;
                    }
                default:
                    this.Geometry = this.CreateGeometry(resourceCreator, Vector2.Zero, false, false);
                    break;
            }
        }

        /// <summary>
        /// (Begin + First + Anchors + Last + End).
        /// </summary>        
        public void BuildGeometry(ICanvasResourceCreator resourceCreator, Vector2 position, bool isSmooth)
        {
            this.Geometry?.Dispose();

            switch (base.Count)
            {
                case 0:
                    break;
                case 1:
                    {
                        // Counterclockwise
                        CanvasPathBuilder pathBuilder = new CanvasPathBuilder(resourceCreator);

                        // 0. Begin
                        Vector2 beginPoint = base[0].Point;
                        pathBuilder.BeginFigure(beginPoint, default);

                        // 3. Last
                        pathBuilder.AddLine(position);

                        pathBuilder.EndFigure(CanvasFigureLoop.Open);
                        this.Geometry = CanvasGeometry.CreatePath(pathBuilder);
                        break;
                    }
                case 2:
                    {
                        // Counterclockwise
                        CanvasPathBuilder pathBuilder = new CanvasPathBuilder(resourceCreator);

                        // Vector2 previousLeftControlPoint;
                        Vector2 previousRightControlPoint;


                        // 0. Begin
                        Anchor begin = base[0];
                        Vector2 beginPoint = begin.Point;

                        pathBuilder.BeginFigure(beginPoint, default);

                        // previousLeftControlPoint = beginPoint;
                        previousRightControlPoint = beginPoint;


                        // 1. First
                        Anchor first = base[1];
                        Vector2 firstPoint = first.Point;

                        if (first.IsSmooth is false && begin.IsSmooth is false)
                        {
                            pathBuilder.AddLine(firstPoint);

                            // previousLeftControlPoint = point;
                            previousRightControlPoint = firstPoint;
                        }
                        else
                        {
                            Vector2 vector = position - (firstPoint + begin.Point) / 2;

                            float left = (firstPoint - (firstPoint + begin.Point) / 2).Length();
                            float right = (firstPoint - position).Length();
                            float length = left + right;

                            Vector2 leftControlPoint = firstPoint - System.Math.Min(left, right) / length * vector;
                            Vector2 rightControlPoint = firstPoint + right / length / 2 * vector;

                            pathBuilder.AddCubicBezier(previousRightControlPoint, leftControlPoint, firstPoint);

                            // previousLeftControlPoint = leftControlPoint;
                            previousRightControlPoint = rightControlPoint;
                        }


                        // 3. Last
                        {
                            Anchor current = base[base.Count - 1];
                            Anchor previous = base[base.Count - 2];
                            Vector2 point = current.Point;
                            Vector2 vector = position - previous.Point;

                            float left = (point - previous.Point).Length();
                            float right = (point - position).Length();
                            float length = left + right;

                            Vector2 leftControlPoint = point - System.Math.Min(left, right) / length / 2 * vector;
                            Vector2 rightControlPoint = point + right / length / 2 * vector;

                            if (current.IsSmooth is false && previous.IsSmooth is false)
                                pathBuilder.AddLine(point);
                            else
                                pathBuilder.AddCubicBezier(previousRightControlPoint, leftControlPoint, point);


                            // 4. End
                            if (isSmooth is false && current.IsSmooth is false)
                                pathBuilder.AddLine(position);
                            else
                                pathBuilder.AddCubicBezier(rightControlPoint, position, position);
                        }


                        pathBuilder.EndFigure(CanvasFigureLoop.Open);
                        this.Geometry = CanvasGeometry.CreatePath(pathBuilder);
                        break;
                    }
                default:
                    this.Geometry = this.CreateGeometry(resourceCreator, position, isSmooth, true);
                    break;
            }
        }

        /// <summary>
        /// Creates a new geometry.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <returns> The created geometry. </returns>
        private CanvasGeometry CreateGeometry(ICanvasResourceCreator resourceCreator, Vector2 position, bool isSmooth, bool hasPosition)
        {
            // Counterclockwise
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(resourceCreator);

            // Vector2 previousLeftControlPoint;
            Vector2 previousRightControlPoint;


            // 0. Begin
            Anchor begin = base[0];
            Vector2 beginPoint = begin.Point;

            pathBuilder.BeginFigure(beginPoint, default);

            // previousLeftControlPoint = beginPoint;
            previousRightControlPoint = beginPoint;


            // 1. First
            Anchor first = base[1];
            Vector2 firstPoint = first.Point;

            if (first.IsSmooth is false && begin.IsSmooth is false)
            {
                pathBuilder.AddLine(firstPoint);

                // previousLeftControlPoint = point;
                previousRightControlPoint = firstPoint;
            }
            else
            {
                // 1. First
                Anchor next = base[2];
                Vector2 vector = next.Point - (firstPoint + begin.Point) / 2;

                float left = (firstPoint - (firstPoint + begin.Point) / 2).Length();
                float right = (firstPoint - next.Point).Length();
                float length = left + right;

                Vector2 leftControlPoint = firstPoint - System.Math.Min(left, right) / length * vector;
                Vector2 rightControlPoint = firstPoint + right / length / 2 * vector;

                pathBuilder.AddCubicBezier(previousRightControlPoint, leftControlPoint, firstPoint);

                // previousLeftControlPoint = leftControlPoint;
                previousRightControlPoint = rightControlPoint;
            }


            // 2. Anchors
            for (int i = 2; i < base.Count - 1; i++)
            {
                Anchor current = base[i];
                Anchor previous = base[i - 1];
                Vector2 point = current.Point;

                if (current.IsSmooth is false && previous.IsSmooth is false)
                {
                    pathBuilder.AddLine(point);

                    // previousLeftControlPoint = point;
                    previousRightControlPoint = point;
                }
                else
                {
                    Anchor next = base[i + 1];
                    Vector2 vector = next.Point - previous.Point;

                    float left = (point - previous.Point).Length();
                    float right = (point - next.Point).Length();
                    float length = left + right;

                    Vector2 leftControlPoint = point - System.Math.Min(left, right) / length / 2 * vector;
                    Vector2 rightControlPoint = point + right / length / 2 * vector;

                    pathBuilder.AddCubicBezier(previousRightControlPoint, leftControlPoint, point);

                    // previousLeftControlPoint = leftControlPoint;
                    previousRightControlPoint = rightControlPoint;
                }
            }


            // 3. Last
            {
                Anchor current = base[base.Count - 1];
                Anchor previous = base[base.Count - 2];
                Vector2 point = current.Point;

                if (hasPosition)
                {
                    Vector2 vector = position - previous.Point;

                    float left = (point - previous.Point).Length();
                    float right = (point - position).Length();
                    float length = left + right;

                    Vector2 leftControlPoint = point - System.Math.Min(left, right) / length / 2 * vector;
                    Vector2 rightControlPoint = point + right / length / 2 * vector;

                    if (current.IsSmooth is false && previous.IsSmooth is false)
                        pathBuilder.AddLine(point);
                    else
                        pathBuilder.AddCubicBezier(previousRightControlPoint, leftControlPoint, point);


                    // 4. End
                    if (isSmooth is false && current.IsSmooth is false)
                        pathBuilder.AddLine(position);
                    else
                        pathBuilder.AddCubicBezier(rightControlPoint, position, position);
                }
                else
                {
                    if (current.IsSmooth is false && previous.IsSmooth is false)
                        pathBuilder.AddLine(point);
                    else
                        pathBuilder.AddCubicBezier(previousRightControlPoint, point, point);
                }
            }


            pathBuilder.EndFigure(CanvasFigureLoop.Open);
            return CanvasGeometry.CreatePath(pathBuilder);
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


        public override string ToString() => "Anchors";

    }
}