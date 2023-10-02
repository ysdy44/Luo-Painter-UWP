using FanKit.Transformers;
using System.Numerics;

namespace Luo_Painter
{
    internal struct TransformMatrix
    {
        public bool IsMove;
        public TransformerMode Mode;

        public Matrix3x2 Matrix;
        public TransformerBorder Border;

        public Transformer StartingTransformer;
        public Transformer Transformer;
    }
}