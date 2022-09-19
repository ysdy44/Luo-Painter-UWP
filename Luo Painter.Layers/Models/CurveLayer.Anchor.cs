using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Luo_Painter.Layers.Models
{
    public sealed class Anchor : ICacheTransform, IDisposable
    {
        /// <summary> Pressure. </summary>
        public float Pressure { get; set; } = 1;
        /// <summary> The cache of <see cref="Anchor.Pressure"/>. </summary>
        public float StartingPressure { get; private set; } = 1;

        /// <summary> Point. </summary>
        public Vector2 Point { get; set; }
        /// <summary> The cache of <see cref="Anchor.Point"/>. </summary>
        public Vector2 StartingPoint { get; private set; }

        /// <summary> Gets or Sets anchor's IsChecked. </summary>
        public bool IsChecked { get; set; }
        /// <summary> The cache of <see cref="Anchor.IsChecked"/>. </summary>
        public bool StartingIsChecked { get; private set; }

        /// <summary> Gets or Sets whether the anchor is smooth. </summary>
        public bool IsSmooth { get; set; }
        /// <summary> The cache of <see cref="Anchor.IsSmooth"/>. </summary>
        public bool StartingIsSmooth { get; private set; }

        public CanvasGeometry Geometry { get; private set; }
        public float ComputePathLength { get; private set; }

        public IList<Vector3> Strokes { get; } = new List<Vector3>();

        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <returns> The cloned anchor. </returns>
        public Anchor Clone()
        {
            return new Anchor
            {
                Pressure = this.Pressure,
                StartingPressure = this.StartingPressure,

                Point = this.Point,
                StartingPoint = this.StartingPoint,

                IsChecked = this.IsChecked,
                StartingIsChecked = this.StartingIsChecked,

                IsSmooth = this.IsSmooth,
                StartingIsSmooth = this.StartingIsSmooth,
            };
        }
        public Anchor Clone(Vector2 offset)
        {
            return new Anchor
            {
                Pressure = this.Pressure,
                StartingPressure = this.StartingPressure,

                Point = this.Point + offset,
                StartingPoint = this.StartingPoint + offset,

                IsChecked = this.IsChecked,
                StartingIsChecked = this.StartingIsChecked,

                IsSmooth = this.IsSmooth,
                StartingIsSmooth = this.StartingIsSmooth,
            };
        }
        public Anchor Clone(Matrix3x2 matrix)
        {
            return new Anchor
            {
                Pressure = this.Pressure,
                StartingPressure = this.StartingPressure,

                Point = Vector2.Transform(this.Point, matrix),
                StartingPoint = Vector2.Transform(this.StartingPoint, matrix),

                IsChecked = this.IsChecked,
                StartingIsChecked = this.StartingIsChecked,

                IsSmooth = this.IsSmooth,
                StartingIsSmooth = this.StartingIsSmooth,
            };
        }


        /// <summary>
        /// <see cref="CanvasPathBuilder.AddLine(Vector2)"/>
        /// </summary>
        public void AddLine(ICanvasResourceCreator resourceCreator, Vector2 point, float strokeWidth)
        {
            this.Dispose();

            this.Geometry = this.Line(resourceCreator, point);
            this.ComputePathLength = Vector2.Distance(this.Point, point);

            if (this.Pressure == 1)
                this.StrokeLine(point, strokeWidth);
            else
                this.StrokeLine(point, 1, strokeWidth);
        }
        /// <summary>
        /// <see cref="CanvasPathBuilder.AddLine(Vector2)"/>
        /// </summary>
        public void AddLine(ICanvasResourceCreator resourceCreator, Anchor anchor, float strokeWidth)
        {
            this.Dispose();

            this.Geometry = this.Line(resourceCreator, anchor.Point);
            this.ComputePathLength = Vector2.Distance(this.Point, anchor.Point);

            if (this.Pressure == anchor.Pressure)
                this.StrokeLine(anchor.Point, strokeWidth);
            else
                this.StrokeLine(anchor.Point, anchor.Pressure, strokeWidth);
        }

        /// <summary>
        /// <see cref="CanvasPathBuilder.AddCubicBezier(Vector2, Vector2, Vector2)"/>
        /// </summary>
        public void AddCubicBezier(ICanvasResourceCreator resourceCreator, Vector2 rightControlPoint, Vector2 leftControlPoint, Vector2 point, float strokeWidth)
        {
            this.Dispose();

            this.Geometry = this.CubicBezier(resourceCreator, rightControlPoint, leftControlPoint, point);
            this.ComputePathLength = this.Geometry.ComputePathLength();

            if (this.Pressure == 1)
                this.StrokeCubicBezier(strokeWidth);
            else
                this.StrokeCubicBezier(1, strokeWidth);
        }
        /// <summary>
        /// <see cref="CanvasPathBuilder.AddCubicBezier(Vector2, Vector2, Vector2)"/>
        /// </summary>
        public void AddCubicBezier(ICanvasResourceCreator resourceCreator, Vector2 rightControlPoint, Vector2 leftControlPoint, Anchor anchor, float strokeWidth)
        {
            this.Dispose();

            this.Geometry = this.CubicBezier(resourceCreator, rightControlPoint, leftControlPoint, anchor.Point);
            this.ComputePathLength = this.Geometry.ComputePathLength();

            if (this.Pressure == anchor.Pressure)
                this.StrokeCubicBezier(strokeWidth);
            else
                this.StrokeCubicBezier(anchor.Pressure, strokeWidth);
        }

        /// <summary>
        /// <see cref="CanvasPathBuilder.AddCubicBezier(Vector2, Vector2, Vector2)"/>
        /// </summary>
        public void AddCubicBezier(ICanvasResourceCreator resourceCreator, Vector2 rightControlPoint, Vector2 point, float strokeWidth)
        {
            this.Dispose();

            this.Geometry = this.CubicBezier(resourceCreator, rightControlPoint, point, point);
            this.ComputePathLength = this.Geometry.ComputePathLength();

            if (this.Pressure == 1)
                this.StrokeCubicBezier(strokeWidth);
            else
                this.StrokeCubicBezier(1, strokeWidth);
        }
        /// <summary>
        /// <see cref="CanvasPathBuilder.AddCubicBezier(Vector2, Vector2, Vector2)"/>
        /// </summary>
        public void AddCubicBezier(ICanvasResourceCreator resourceCreator, Vector2 rightControlPoint, Anchor anchor, float strokeWidth)
        {
            this.Dispose();

            this.Geometry = this.CubicBezier(resourceCreator, rightControlPoint, anchor.Point, anchor.Point);
            this.ComputePathLength = this.Geometry.ComputePathLength();

            if (this.Pressure == anchor.Pressure)
                this.StrokeCubicBezier(strokeWidth);
            else
                this.StrokeCubicBezier(anchor.Pressure, strokeWidth);
        }


        private void StrokeLine(Vector2 point, float strokeWidth)
        {
            float sw = this.Pressure * strokeWidth / 2;
            float swHalf = sw / 2;

            // 1.Head
            this.Strokes.Add(new Vector3(this.Point, sw));

            // 2.Body
            for (float i = swHalf; i < this.ComputePathLength; i += swHalf)
            {
                float pect = i / this.ComputePathLength;

                this.Strokes.Add(new Vector3(Vector2.Lerp(this.Point, point, pect), sw));
            }
        }
        private void StrokeLine(Vector2 point, float pressure, float strokeWidth)
        {
            // 1.Head
            float sw2 = System.Math.Max(0.4f, this.Pressure * strokeWidth / 2);

            this.Strokes.Add(new Vector3(this.Point, sw2));
            float i = sw2 / 2;

            // 2.Body
            do
            {
                float pect = i / this.ComputePathLength;
                float sw = System.Math.Max(0.4f, (pect * pressure + (1 - pect) * this.Pressure) * strokeWidth / 2);

                this.Strokes.Add(new Vector3(Vector2.Lerp(this.Point, point, pect), sw));
                i += sw / 2;
            } while (i < this.ComputePathLength);
        }

        private void StrokeCubicBezier(float strokeWidth)
        {
            float sw = this.Pressure * strokeWidth / 2;
            float swHalf = sw / 2;

            // 1.Head
            this.Strokes.Add(new Vector3(this.Point, sw));

            // 2.Body
            for (float i = swHalf; i < this.ComputePathLength; i += swHalf)
            {
                this.Strokes.Add(new Vector3(this.Geometry.ComputePointOnPath(i), sw));
            }
        }
        private void StrokeCubicBezier(float pressure, float strokeWidth)
        {
            // 1.Head
            float sw2 = System.Math.Max(0.4f, this.Pressure * strokeWidth / 2);

            this.Strokes.Add(new Vector3(this.Point, sw2));
            float i = sw2 / 2;

            // 2.Body
            do
            {
                float pect = i / this.ComputePathLength;
                float sw = System.Math.Max(0.4f, (pect * pressure + (1 - pect) * this.Pressure) * strokeWidth / 2);

                this.Strokes.Add(new Vector3(this.Geometry.ComputePointOnPath(i), sw));
                i += sw / 2;
            } while (i < this.ComputePathLength);
        }


        readonly Vector2[] LinePoints = new Vector2[2];
        internal CanvasGeometry Line(ICanvasResourceCreator resourceCreator, Vector2 point)
        {
            this.LinePoints[0] = this.Point;
            this.LinePoints[1] = point;
            return CanvasGeometry.CreatePolygon(resourceCreator, this.LinePoints);
        }
        internal CanvasGeometry CubicBezier(ICanvasResourceCreator resourceCreator, Vector2 rightControlPoint, Vector2 leftControlPoint, Vector2 point)
        {
            using (CanvasPathBuilder pathBuilder = new CanvasPathBuilder(resourceCreator))
            {
                pathBuilder.BeginFigure(this.Point, default);

                pathBuilder.AddCubicBezier(rightControlPoint, leftControlPoint, point);

                pathBuilder.EndFigure(CanvasFigureLoop.Open);
                return CanvasGeometry.CreatePath(pathBuilder);
            }
        }


        //@Static
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector2 CubicBezier(Vector2 point, Vector2 previousPoint, Vector2 nextPoint, ref Vector2 rightControlPoint)
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
        internal static Vector2 CubicBezierFirst(Vector2 firstPoint, Vector2 beginPoint, Vector2 nextPoint, ref Vector2 rightControlPoint)
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
        /// Cache the anchor's transformer.
        /// </summary>
        public void CacheTransform()
        {
            this.StartingPressure = this.Pressure;

            this.StartingPoint = this.Point;

            this.StartingIsChecked = this.IsChecked;
            this.StartingIsSmooth = this.IsSmooth;
        }
        /// <summary>
        /// Transforms the anchor by the given vector.
        /// </summary>
        /// <param name="vector"> The add value use to summed. </param>
        public void TransformAdd(Vector2 vector)
        {
            this.Point = this.StartingPoint + vector;
        }
        /// <summary>
        /// Transforms the anchor by the given matrix.
        /// </summary>
        /// <param name="matrix"> The resulting matrix. </param>
        public void TransformMultiplies(Matrix3x2 matrix)
        {
            this.Point = Vector2.Transform(this.StartingPoint, matrix);
        }


        /// <summary>
        /// The vector was contained in the rectangle.
        /// </summary>
        /// <param name="left"> The destination rectangle's left. </param>
        /// <param name="top"> The destination rectangle's top. </param>
        /// <param name="right"> The destination rectangle's right. </param>
        /// <param name="bottom"> The destination rectangle's bottom. </param>
        /// <returns> Return **true** if the vector was contained in rectangle, otherwise **false**. </returns>
        public bool Contained(float left, float top, float right, float bottom)
        {
            if (this.Point.X < left) return false;
            if (this.Point.Y < top) return false;
            if (this.Point.X > right) return false;
            if (this.Point.Y > bottom) return false;

            return true;
        }
        /// <summary>
        /// The vector was contained in a rectangle.
        /// </summary>
        /// <param name="transformerRect"> The destination rectangle. </param>
        /// <returns> Return **true** if the vector was contained in rectangle. </returns>
        public bool Contained(TransformerRect transformerRect) => this.Contained(transformerRect.Left, transformerRect.Top, transformerRect.Right, transformerRect.Bottom);

        public void Dispose()
        {
            this.Geometry?.Dispose();
            this.Geometry = null;
            this.ComputePathLength = 0;

            this.Strokes.Clear();
        }
    }
}