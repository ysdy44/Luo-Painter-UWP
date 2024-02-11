using FanKit.Transformers;
using System.Numerics;

namespace Luo_Painter.HSVColorPickers
{
    public struct TransformMatrix
    {
        public bool IsMove;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        // Source
        public float X;
        public float Y;
        public float Width;
        public float Height;
        // Destination
        public Transformer StartingTransformer;
        public Transformer Transformer;

        public void UpdateMatrix()
        {
            this.Matrix = Transformer.FindHomography(this.X, this.Y, this.Width, this.Height, this.Transformer);
        }
    }
}