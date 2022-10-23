using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Brushes
{
    /// <summary>
    /// <see cref="InkDrawingAttributes"/>
    /// </summary>
    public partial class InkAttributes
    {

        /// <summary>
        /// <see cref=" InkType.None"/> <para/>
        /// <see cref=" InkType.General"/> <para/>
        /// <see cref=" InkType.Tip"/> <para/>
        /// <see cref=" InkType.Line"/> <para/>
        /// <see cref=" InkType.Blur"/> <para/>
        /// <see cref=" InkType.Mosaic"/> <para/>
        /// <see cref=" InkType.Erase"/> <para/>
        /// <see cref=" InkType.Liquefy"/> <para/>
        /// </summary>
        public InkType Type { get; set; } = InkType.General; /// <see cref="InkPresenter.GetType"/>

        /// <summary>
        /// <see cref=" InkType.None"/> <para/>
        /// <see cref=" InkType.Blend"/> <para/>
        /// <see cref=" InkType.Mix"/> <para/>
        /// </summary>
        public InkType Mode { get; set; } = InkType.None; /// <see cref="InkPresenter.GetType"/>


        public float Size { get; set; }
        public float Opacity { get; set; } /// <see cref="InkPresenter.GetType"/>

        public float Spacing { get; set; }
        public float Flow { get; set; }


        public bool IgnoreSizePressure { get; set; }
        public bool IgnoreFlowPressure { get; set; }

        public PenTipShape Tip { get; set; }
        public bool IsStroke { get; set; }


        public BlendEffectMode BlendMode { get; set; }
        public BrushEdgeHardness Hardness { get; set; }


        public bool Rotate { get; set; }
        public int Step { get; set; } = 1024;

        public string Shape { get; set; }
        public string Grain { get; set; }


        public void CopyWith(InkAttributes attributes)
        {
            this.Type = attributes.Type;
            this.Mode = attributes.Mode;

            this.Size = attributes.Size;
            this.Opacity = attributes.Opacity;
            this.Spacing = attributes.Spacing;
            this.Flow = attributes.Flow;

            this.IgnoreSizePressure = attributes.IgnoreSizePressure;
            this.IgnoreFlowPressure = attributes.IgnoreFlowPressure;

            this.Tip = attributes.Tip;
            this.IsStroke = attributes.IsStroke;

            this.BlendMode = attributes.BlendMode;
            this.Hardness = attributes.Hardness;

            this.Rotate = attributes.Rotate;
            this.Step = attributes.Step;

            this.Shape = attributes.Shape;
            this.Grain = attributes.Grain;
        }

    }
}