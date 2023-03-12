using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;

namespace Luo_Painter.Layers
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
            Source = source;
            Size.X = Width = (int)source.SizeInPixels.Width;
            Size.Y = Height = (int)source.SizeInPixels.Height;
            Length = Size.Length();
        }

        public void Draw(CanvasDrawingSession ds, Matrix3x2 matrix)
        {
            Matrix3x2 m = Matrix3x2.CreateTranslation(Position) * matrix;
            ds.DrawImage(new Transform2DEffect
            {
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                TransformMatrix = Matrix3x2.CreateScale(Scale) * m,
                Source = Source
            });
            ds.DrawNode3(Vector2.Transform(Size, m));
        }

        public void Cache() => StartingPosition = Position;
        public void Add(Vector2 vector) => Position = StartingPosition + vector;

        public void Resizing(Vector2 position)
        {
            if (Position.X > position.X)
                Scale = 0.1f;
            else if (Position.Y > position.Y)
                Scale = 0.1f;
            else
            {
                float length = Vector2.Distance(Position, position);
                Scale = Math.Clamp(length / Length, 0.1f, 10f);
            }

            Size.X = Width * Scale;
            Size.Y = Height * Scale;
        }

        public bool Contains(Vector2 p) => Contains((int)((p.X - Position.X) / Scale), (int)((p.Y - Position.Y) / Scale));
        private bool Contains(int x, int y)
        {
            if (x < 0) return false;
            if (y < 0) return false;
            if (x >= Width) return false;
            if (y >= Height) return false;
            return true;
        }

        public void Dispose() => Source.Dispose();
    }
}