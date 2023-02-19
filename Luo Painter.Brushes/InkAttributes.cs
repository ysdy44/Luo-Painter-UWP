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

        public uint Tile { get; set; } = 0000000000;

        // Property
        public float Size { get; set; }
        public float Opacity { get; set; } /// <see cref="InkPresenter.GetType"/>
        public float Spacing { get; set; }
        public float Flow { get; set; } /// <see cref="InkPresenter.GetType"/>

        public float MinSize { get; set; }
        public float MinFlow { get; set; }

        public BrushEasePressure SizePressure { get; set; }
        public BrushEasePressure FlowPressure { get; set; }

        public PenTipShape Tip { get; set; }
        public bool IsStroke { get; set; }

        public BrushEdgeHardness Hardness { get; set; }

        // Texture
        public bool Rotate { get; set; }
        public string Shape { get; set; }  /// <see cref="InkPresenter.GetType"/>
        public bool RecolorShape { get; set; } = true;

        public float GrainScale { get; set; } = 1;
        public string Grain { get; set; }  /// <see cref="InkPresenter.GetType"/>
        public bool RecolorGrain { get; set; } = true;

        public BlendEffectMode BlendMode { get; set; } = (BlendEffectMode)(-1); /// <see cref="InkPresenter.GetType"/>

        // Mix
        public float Mix { get; set; } /// <see cref="InkPresenter.GetType"/>
        public float Wet { get; set; } = 10;
        public float Persistence { get; set; }

        public void CopyWith(InkAttributes attributes)
        {
            this.Type = attributes.Type;

            // Property
            this.Size = attributes.Size;
            this.Opacity = attributes.Opacity;
            this.Spacing = attributes.Spacing;
            this.Flow = attributes.Flow;

            this.MinSize = attributes.MinSize;
            this.MinFlow = attributes.MinFlow;

            this.SizePressure = attributes.SizePressure;
            this.FlowPressure = attributes.FlowPressure;

            this.Tip = attributes.Tip;
            this.IsStroke = attributes.IsStroke;

            this.Hardness = attributes.Hardness;

            // Texture
            this.Rotate = attributes.Rotate;
            this.Shape = attributes.Shape;
            this.RecolorShape = attributes.RecolorShape;

            this.GrainScale = attributes.GrainScale;
            this.Grain = attributes.Grain;
            this.RecolorGrain = attributes.RecolorGrain;

            this.BlendMode = attributes.BlendMode;

            // Mix
            this.Mix = attributes.Mix;
            this.Wet = attributes.Wet;
            this.Persistence = attributes.Persistence;
        }

    }
}