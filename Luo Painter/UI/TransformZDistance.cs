using FanKit.Transformers;
using System.Numerics;

namespace Luo_Painter
{
    internal struct TransformZDistance
    {
        public Vector2 Distance;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        public TransformerBorder Border;

        public Transformer Transformer;
    }
}