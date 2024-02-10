using FanKit.Transformers;
using System.Numerics;

namespace Luo_Painter.HSVColorPickers
{
    public struct TransformMatrix3D
    {
        public Vector2 Distance;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        // Source
        public TransformerBorder Border;
        // Destination
        public Transformer Transformer;

        public void UpdateMatrix()
        {
            this.Matrix = Transformer.FindHomography(this.Transformer, this.Border, out this.Distance);
        }
    }
}