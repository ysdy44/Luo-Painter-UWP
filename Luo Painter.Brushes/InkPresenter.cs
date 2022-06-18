using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        public float Size { get; set; } = 22f;
        public float Opacity { get; set; } = 1f;
        public float Spacing { get; set; } = 0.25f;
        public BrushEdgeHardness Hardness { get; set; }

        public bool Rotate { get; set; }
        public int Step { get; set; } = 1024;

        public BlendEffectMode BlendMode { get; set; }

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

        public void SetMask(bool allowMask, CanvasBitmap mask = null)
        {
            this.AllowMask = allowMask;

            if (mask is null) return;

            if (this.Mask is null)
                this.Mask = mask;
            else
            {
                this.Mask.Dispose();
                this.Mask = mask;
            }
        }

        public void SetPattern(bool allowPattern, CanvasBitmap pattern = null)
        {
            this.AllowPattern = allowPattern;

            if (pattern is null) return;

            if (this.Pattern is null)
                this.Pattern = pattern;
            else
            {
                this.Pattern.Dispose();
                this.Pattern = pattern;
            }
        }

    }
}