using FanKit.Transformers;
using System.Numerics;

namespace Luo_Painter.HSVColorPickers
{
    public struct TransformMatrix3D
    {
        public TransformerMode Mode;

        public Matrix4x4 Matrix;
        // Source
        public float X;
        public float Y;
        public float Width;
        public float Height;
        // Destination
        public Transformer Transformer;

        public void UpdateMatrix()
        {
            this.Matrix = Transformer.FindHomography3D(this.X, this.Y, this.Width, this.Height, this.Transformer);
        }
    }
}