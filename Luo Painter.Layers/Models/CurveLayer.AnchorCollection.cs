using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Luo_Painter.Layers.Models
{
    public sealed class AnchorCollection : List<Anchor>
    {

        public int Index = -1;
        public Anchor SelectedItem => (this.Index is -1) ? null : base[this.Index];


        /// <summary>
        /// (Begin + First + Anchors + Last).
        /// </summary> 
        public void BuildGeometry(ICanvasResourceCreator resourceCreator)
        {
            switch (base.Count)
            {
                case 0:
                    break;
                case 1:
                    this.Single().Dispose();
                    break;
                case 2:
                    this.First().AddLine(resourceCreator, this.Last().Point);
                    this.Last().Dispose();
                    break;
                default:
                    this.CreateGeometry(resourceCreator, Vector2.Zero, false, false);
                    break;
            }
        }
        /// <summary>
        /// (Begin + First + Anchors + Last + End).
        /// </summary>        
        public void BuildGeometry(ICanvasResourceCreator resourceCreator, Vector2 position, bool isSmooth)
        {
            switch (base.Count)
            {
                case 0:
                    break;
                case 1:
                    // Begin + End
                    this.First().AddLine(resourceCreator, position);
                    break;
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
                            begin.AddLine(resourceCreator, firstPoint);
                            previousRightControlPoint = firstPoint;
                        }
                        else
                        {
                            Vector2 leftControlPoint = AnchorCollection.CubicBezierFirst(firstPoint, beginPoint, position, ref previousRightControlPoint);
                            begin.AddCubicBezier(resourceCreator, beginPoint, leftControlPoint, firstPoint);
                        }


                        // 4. End
                        Anchor last = base[base.Count - 1];

                        if (isSmooth is false && last.IsSmooth is false)
                            first.AddLine(resourceCreator, position);
                        else
                            first.AddCubicBezier(resourceCreator, previousRightControlPoint, position);
                    }
                    break;
                default:
                    this.CreateGeometry(resourceCreator, position, isSmooth, true);
                    break;
            }
        }

        /// <summary>
        /// Creates a new geometry.
        /// </summary>
        /// <param name="resourceCreator"> The resource-creator. </param>
        /// <returns> The created geometry. </returns>
        private void CreateGeometry(ICanvasResourceCreator resourceCreator, Vector2 position, bool isSmooth, bool hasPosition)
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
                begin.AddLine(resourceCreator, firstPoint);
                previousRightControlPoint = firstPoint;
            }
            else
            {
                Anchor next = base[2];
                Vector2 nextPoint = next.Point;

                Vector2 leftControlPoint = AnchorCollection.CubicBezierFirst(firstPoint, beginPoint, nextPoint, ref previousRightControlPoint);
                begin.AddCubicBezier(resourceCreator, beginPoint, leftControlPoint, firstPoint);
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
                    previous.AddLine(resourceCreator, point);
                    previousRightControlPoint = point;
                }
                else
                {
                    Anchor next = base[i + 1];
                    Vector2 nextPoint = next.Point;

                    Vector2 rightControlPoint = previousRightControlPoint;
                    Vector2 leftControlPoint = AnchorCollection.CubicBezier(point, previousPoint, nextPoint, ref previousRightControlPoint);
                    previous.AddCubicBezier(resourceCreator, rightControlPoint, leftControlPoint, point);
                }
            }


            if (hasPosition)
            {
                // 3. Last
                Anchor current = base[base.Count - 1];
                Anchor previous = base[base.Count - 2];
                Vector2 point = current.Point;
                Vector2 previousPoint = previous.Point;

                Vector2 rightControlPoint = previousRightControlPoint;
                Vector2 leftControlPoint = AnchorCollection.CubicBezier(point, previousPoint, position, ref previousRightControlPoint);

                if (current.IsSmooth is false && previous.IsSmooth is false)
                    previous.AddLine(resourceCreator, point);
                else
                    previous.AddCubicBezier(resourceCreator, rightControlPoint, leftControlPoint, point);


                // 4. End
                if (isSmooth is false && current.IsSmooth is false)
                    current.AddLine(resourceCreator, position);
                else
                    current.AddCubicBezier(resourceCreator, previousRightControlPoint, position);
            }
            else
            {
                // 3. Last
                Anchor current = base[base.Count - 1];
                Anchor previous = base[base.Count - 2];
                Vector2 point = current.Point;

                if (current.IsSmooth is false && previous.IsSmooth is false)
                    previous.AddLine(resourceCreator, point);
                else
                    previous.AddCubicBezier(resourceCreator, previousRightControlPoint, point);

                current.Dispose();
            }
        }


        //@Static
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 CubicBezier(Vector2 point, Vector2 previousPoint, Vector2 nextPoint, ref Vector2 rightControlPoint)
        {
            Vector2 vector = nextPoint - previousPoint;

            float left = (point - previousPoint).Length();
            float right = (point - nextPoint).Length();
            float length = left + right;

            Vector2 leftControlPoint = point - System.Math.Min(left, right) / length / 2 * vector;
            rightControlPoint = point + right / length / 2 * vector;

            return leftControlPoint;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 CubicBezierFirst(Vector2 firstPoint, Vector2 beginPoint, Vector2 nextPoint, ref Vector2 rightControlPoint)
        {
            Vector2 vector = nextPoint - (firstPoint + beginPoint) / 2;

            float left = (firstPoint - (firstPoint + beginPoint) / 2).Length();
            float right = (firstPoint - nextPoint).Length();
            float length = left + right;

            Vector2 leftControlPoint = firstPoint - System.Math.Min(left, right) / length * vector;
            rightControlPoint = firstPoint + right / length / 2 * vector;

            return leftControlPoint;
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