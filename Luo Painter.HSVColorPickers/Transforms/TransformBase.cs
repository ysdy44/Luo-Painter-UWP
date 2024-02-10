using FanKit.Transformers;

namespace Luo_Painter.UI
{
    public struct TransformBase
    {
        public bool IsMove;
        public TransformerMode Mode;

        public Transformer StartingTransformer;
        public Transformer Transformer;
    }
}