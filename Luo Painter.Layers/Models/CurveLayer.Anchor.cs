using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Luo_Painter.Layers.Models
{
    public sealed class Anchor : Node, IDisposable
    {
        /// <summary> Pressure. </summary>
        public float Pressure { get; set; } = 1;
        /// <summary> The cache of <see cref="Anchor.Pressure"/>. </summary>
        public float StartingPressure { get; private set; } = 1;

        public CanvasGeometry Geometry { get; private set; }
        public float ComputePathLength { get; private set; }

        public IList<Vector3> Strokes { get; } = new List<Vector3>();

        /// <summary>
        /// Get own copy.
        /// </summary>
        /// <returns> The cloned anchor. </returns>
        public new Anchor Clone()
        {
            return new Anchor
            {
                Pressure = this.Pressure,

                Point = this.Point,
                LeftControlPoint = this.LeftControlPoint,
                RightControlPoint = this.RightControlPoint,

                IsChecked = this.IsChecked,
                IsSmooth = this.IsSmooth,
            };
        }
        public Anchor Clone(Vector2 offset)
        {
            return new Anchor
            {
                Pressure = this.Pressure,

                Point = this.Point + offset,
                LeftControlPoint = this.LeftControlPoint + offset,
                RightControlPoint = this.RightControlPoint + offset,

                IsChecked = this.IsChecked,
                IsSmooth = this.IsSmooth,
            };
        }
        public Anchor Clone(Matrix3x2 matrix)
        {
            return new Anchor
            {
                Pressure = this.Pressure,
                StartingPressure = this.StartingPressure,

                Point = Vector2.Transform(this.Point, matrix),
                LeftControlPoint = Vector2.Transform(this.LeftControlPoint, matrix),
                RightControlPoint = Vector2.Transform(this.RightControlPoint, matrix),

                IsChecked = this.IsChecked,
                IsSmooth = this.IsSmooth,
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


        public void Dispose()
        {
            this.Geometry?.Dispose();
            this.Geometry = null;
            this.ComputePathLength = 0;

            this.Strokes.Clear();
        }
    }
}