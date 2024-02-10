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
        public TransformerBorder Border;
        // Destination
        public Transformer StartingTransformer;
        public Transformer Transformer;

        public void UpdateMatrix()
        {
            this.Matrix = Transformer.FindHomography(this.Border, this.Transformer);
        }
    }
}