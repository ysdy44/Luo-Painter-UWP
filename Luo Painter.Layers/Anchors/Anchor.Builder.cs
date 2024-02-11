using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;

namespace Luo_Painter.Layers
{
    partial class Anchor
    {
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
    }
}