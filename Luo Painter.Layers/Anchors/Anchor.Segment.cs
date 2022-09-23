using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public sealed partial class Anchor : Node, IDisposable
    {
        /// <summary>
        /// Don't change this Magic Code !
        /// </summary>
        public void Segment(Vector2 right, Vector2 left)
        {
            Vector2 rightVector = right - this.Point;
            Vector2 leftVector = left - this.Point;

            float leftLength = leftVector.Length();
            float rightLength = rightVector.Length();

            float length = leftLength + rightLength;
            float distance = (left - right).Length() / length / length / 4;

            Vector2 vector =
                Vector2.Normalize(rightVector) * leftLength
                - Vector2.Normalize(leftVector) * rightLength
                + right - left;

            this.LeftControlPoint = this.Point - leftLength * distance * vector;
            this.RightControlPoint = this.Point + rightLength * distance * vector;
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
    }
}