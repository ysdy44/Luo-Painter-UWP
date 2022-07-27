using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;

namespace Luo_Painter.Blends
{
    public sealed class ReferenceImage : IDisposable
    {

        Vector2 StartingPosition;
        public Vector2 Position;

        public float Scale = 1;
        public Vector2 Size;

        private readonly float Length;
        public readonly int Width;
        public readonly int Height;

        private readonly CanvasBitmap Source;

        public ReferenceImage(CanvasBitmap source)
        {
            this.Source = source;
            this.Size.X = this.Width = (int)source.SizeInPixels.Width;
            this.Size.Y = this.Height = (int)source.SizeInPixels.Height;
            this.Length = this.Size.Length();
        }

        public void Draw(CanvasDrawingSession ds, Matrix3x2 matrix)
        {
            Matrix3x2 m = Matrix3x2.CreateTranslation(this.Position) * matrix;
            ds.DrawImage(new Transform2DEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                TransformMatrix = Matrix3x2.CreateScale(this.Scale) * m,
                Source = this.Source
            });
            ds.DrawNode3(Vector2.Transform(this.Size, m));
        }

        public void Cache() => this.StartingPosition = this.Position;
        public void Add(Vector2 vector) => this.Position = this.StartingPosition + vector;

        public void Resizing(Vector2 position)
        {
            if (this.Position.X > position.X)
                this.Scale = 0.1f;
            else if (this.Position.Y > position.Y)
                this.Scale = 0.1f;
            else
            {
                float length = Vector2.Distance(this.Position, position);
                this.Scale = System.Math.Clamp(length / this.Length, 0.1f, 10f);
            }

            this.Size.X = this.Width * this.Scale;
            this.Size.Y = this.Height * this.Scale;
        }

        public bool Contains(Vector2 p) => this.Contains((int)((p.X - this.Position.X) / this.Scale), (int)((p.Y - this.Position.Y) / this.Scale));
        private bool Contains(int x, int y)
        {
            if (x < 0) return false;
            if (y < 0) return false;
            if (x >= this.Width) return false;
            if (y >= this.Height) return false;
            return true;
        }

        public void Dispose() => this.Source.Dispose();
    }
}