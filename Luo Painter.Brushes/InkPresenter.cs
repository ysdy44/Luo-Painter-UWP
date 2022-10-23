using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter : InkAttributes<float>
    {
        public bool AllowShape { get; private set; } // GetType
        public CanvasBitmap ShapeSource { get; private set; }

        public bool AllowGrain { get; private set; }
        public CanvasBitmap GrainSource { get; private set; }

        public void Construct(InkAttributes<float> brush)
        {
            this.Type = brush.Type;
            this.Mode = brush.Mode;

            this.Size = (float)brush.Size;
            this.Opacity = (float)brush.Opacity;
            this.Spacing = (float)brush.Spacing;
            this.Flow = (float)brush.Flow;

            this.IgnoreSizePressure = brush.IgnoreSizePressure;
            this.IgnoreFlowPressure = brush.IgnoreFlowPressure;

            this.Tip = brush.Tip;
            this.IsStroke = brush.IsStroke;

            this.BlendMode = brush.BlendMode;
            this.Hardness = brush.Hardness;

            this.Rotate = brush.Rotate;
            this.Step = brush.Step;

            this.Shape = brush.Shape;
            this.Grain = brush.Grain;
        }
    }
}