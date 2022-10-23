using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes
    {
        public bool AllowShape { get; private set; } // GetType
        public CanvasBitmap ShapeSource { get; private set; }

        public bool AllowGrain { get; private set; }
        public CanvasBitmap GrainSource { get; private set; }

        public void Construct(InkAttributes brush)
        {
            base.CopyWith(brush);
        }
    }
}