using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;

namespace Luo_Painter.Layers
{
    public sealed partial class Anchor 
    {
        // Don't change this Magic Code !
        internal void SegmentLine()
        {
            base.LeftControlPoint = base.Point;
            base.RightControlPoint = base.Point;
        }
        // Don't change this Magic Code !
        internal void Segment(Vector2 right, Vector2 left)
        {
            Vector2 rightVector = right - base.Point;
            Vector2 leftVector = left - base.Point;

            float leftLength = leftVector.Length();
            float rightLength = rightVector.Length();

            float length = leftLength + rightLength;
            float distance = (left - right).Length() / length / length / 4;

            Vector2 vector = right - left
                + Vector2.Normalize(rightVector) * leftLength
                - Vector2.Normalize(leftVector) * rightLength;

            base.LeftControlPoint = base.Point - leftLength * distance * vector;
            base.RightControlPoint = base.Point + rightLength * distance * vector;
        }
        // Don't change this Magic Code !
        internal void SegmentFirst(Vector2 right, Vector2 begin)
        {
            Vector2 left = (base.Point + begin) / 2;

            Vector2 rightVector = right - base.Point;
            Vector2 leftVector = left - base.Point;

            float leftLength = leftVector.Length();
            float rightLength = rightVector.Length();

            float length = leftLength + rightLength;
            float distance = (left - right).Length() / length / length / 4;

            Vector2 vector = right - left
                + Vector2.Normalize(rightVector) * leftLength
                - Vector2.Normalize(leftVector) * rightLength;

            base.LeftControlPoint = base.Point - leftLength * distance * vector * 2.2360679774997896964091736687313f; // √5
            base.RightControlPoint = base.Point + rightLength * distance * vector;
        }

        readonly Vector2[] LinePoints = new Vector2[2];
        private CanvasGeometry Line(ICanvasResourceCreator resourceCreator, Vector2 point)
        {
            this.LinePoints[0] = base.Point;
            this.LinePoints[1] = point;
            return CanvasGeometry.CreatePolygon(resourceCreator, this.LinePoints);
        }
        private CanvasGeometry CubicBezier(ICanvasResourceCreator resourceCreator, Vector2 rightControlPoint, Vector2 leftControlPoint, Vector2 point)
        {
            using (CanvasPathBuilder pathBuilder = new CanvasPathBuilder(resourceCreator))
            {
                pathBuilder.BeginFigure(base.Point, default);

                pathBuilder.AddCubicBezier(rightControlPoint, leftControlPoint, point);

                pathBuilder.EndFigure(CanvasFigureLoop.Open);
                return CanvasGeometry.CreatePath(pathBuilder);
            }
        }

        private void StrokeLine(Vector2 point, float strokeWidth)
        {
            float sw = this.Pressure * strokeWidth / 2;
            float swHalf = sw / 2;

            for (float i = swHalf; i < this.ComputePathLength; i += swHalf)
            {
                float pect = i / this.ComputePathLength;

                this.Strokes.Add(new Vector3(Vector2.Lerp(base.Point, point, pect), sw));
            }
        }
        private void StrokeLine(Vector2 point, float pressure, float strokeWidth)
        {
            float sw2 = System.Math.Max(0.4f, this.Pressure * strokeWidth / 2);
            float i = sw2 / 2;

            do
            {
                float pect = i / this.ComputePathLength;
                float sw = System.Math.Max(0.4f, (pect * pressure + (1 - pect) * this.Pressure) * strokeWidth / 2);

                this.Strokes.Add(new Vector3(Vector2.Lerp(base.Point, point, pect), sw));
                i += sw / 2;
            } while (i < this.ComputePathLength);
        }

        private void StrokeCubicBezier(float strokeWidth)
        {
            float sw = this.Pressure * strokeWidth / 2;
            float swHalf = sw / 2;

            for (float i = swHalf; i < this.ComputePathLength; i += swHalf)
            {
                this.Strokes.Add(new Vector3(this.Geometry.ComputePointOnPath(i), sw));
            }
        }
        private void StrokeCubicBezier(float pressure, float strokeWidth)
        {
            float sw2 = System.Math.Max(0.4f, this.Pressure * strokeWidth / 2);
            float i = sw2 / 2;

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