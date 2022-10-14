using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    public sealed partial class InkPresenter
    {

        /// <summary>
        /// <see cref=" InkType.None"/>
        /// <see cref=" InkType.Brush"/>
        /// <see cref=" InkType.Circle"/>
        /// <see cref=" InkType.Line"/>
        /// <see cref=" InkType.Blur"/>
        /// <see cref=" InkType.Mosaic"/>
        /// <see cref=" InkType.Erase"/>
        /// <see cref=" InkType.Liquefy"/>
        /// </summary>
        public InkType ToolType { get; set; } // GetType

        /// <summary>
        /// <see cref=" InkType.None"/>
        /// <see cref=" InkType.Blend"/>
        /// <see cref=" InkType.Mix"/>
        /// </summary>
        public InkType Mode { get; set; } // GetType


        public float Size { get; set; } = 22f;
        public float Opacity { get; set; } = 1f; // GetType

        public float Spacing { get; set; } = 0.25f;
        public float Flow { get; set; } = 1f;


        public bool IgnoreSizePressure { get; set; }
        public bool IgnoreFlowPressure { get; set; }

        public PenTipShape Shape { get; set; }
        public bool IsStroke { get; set; }


        public BlendEffectMode BlendMode { get; set; }
        public BrushEdgeHardness Hardness { get; set; }


        public bool Rotate { get; set; }
        public int Step { get; set; } = 1024;

        public bool AllowMask { get; private set; } // GetType
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