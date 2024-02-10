using FanKit.Transformers;
using System.Numerics;

namespace Luo_Painter.HSVColorPickers
{
    public struct TransformMatrix3D
    {
        public Vector2 Distance;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        public TransformerBorder Border;

        public Transformer Transformer;
    }
}