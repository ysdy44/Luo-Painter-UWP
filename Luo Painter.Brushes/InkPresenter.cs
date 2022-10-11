using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public float Size { get; set; } = 22f;
        public float Opacity { get; set; } = 1f;

        public float Spacing { get; set; } = 0.25f;
        public float Flow { get; set; } = 1f;

        public PenTipShape Shape { get; set; }
        public bool IsStroke { get; set; }

        public BlendEffectMode BlendMode { get; set; } = (BlendEffectMode)(-1);
        public BrushEdgeHardness Hardness { get; set; }

        public bool Rotate { get; set; }
        public int Step { get; set; } = 1024;

        public bool AllowMask { get; private set; }
        public string MaskTexture { get; private set; }
        public CanvasBitmap Mask { get; private set; }

        public bool AllowPattern { get; private set; }
        public string PatternTexture { get; private set; }
        public CanvasBitmap Pattern { get; private set; }

        public void Construct(PaintBrush brush)
        {
            this.Size = (float)brush.Size;
            this.Opacity = (float)brush.Opacity;
            this.Spacing = (float)brush.Spacing;
            this.Hardness = brush.Hardness;
            this.Rotate = brush.Rotate;
            this.Step = brush.Step;
        }

    }
}