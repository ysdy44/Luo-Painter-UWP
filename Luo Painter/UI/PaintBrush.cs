using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.UI
{
    public sealed class PaintBrush : InkAttributes
    {
        public InkType Type { get; set; } = InkType.General;

        // Property
        public double Size2 { get => base.Size; set => base.Size = (float)value; }
        public double Opacity2 { get => base.Opacity; set => base.Opacity = (float)value; }

        public double Spacing2 { get => base.Spacing; set => base.Spacing = (float)value; }
        public double Flow2 { get => base.Flow; set => base.Flow = (float)value; }

        public double MinSize2 { get => base.MinSize; set => base.MinSize = (float)value; }
        public double MinFlow2 { get => base.MinFlow; set => base.MinFlow = (float)value; }

        public double GrainScale2 { get => base.GrainScale; set => base.GrainScale = (float)value; }

        public uint Tile { get; set; }
        public string ImageSource => this.Tile == default ? BrushExtensions.DeaultTile : this.Tile.GetTile();
        public int Tile2 { get => (int)this.Tile; set => this.Tile = (uint)value; }


        public BrushEasePressure SizePressure { get; set; }
        public BrushEasePressure FlowPressure { get; set; }

        public Windows.UI.Input.Inking.PenTipShape Tip { get; set; }
        public bool IsStroke { get; set; }

        public BrushEdgeHardness Hardness { get; set; }

        // Texture
        public bool Rotate { get; set; }
        public string Shape { get; set; }
        public bool RecolorShape { get; set; } = true;

        //public double GrainScale2 { get; set; } = 1;
        public string Grain { get; set; }
        public bool RecolorGrain { get; set; } = true;

        public BlendEffectMode BlendMode { get; set; } = (BlendEffectMode)(-1);

        // Mix
        public double Mix2 { get; set; }
        public double Wet2 { get; set; } = 10;
        public double Persistence2 { get; set; }

        public void CopyTo(InkPresenter presenter)
        {
            presenter.Type = this.Type;

            // Property
            presenter.Size = this.Size;
            presenter.Opacity = this.Opacity;
            presenter.Spacing = this.Spacing;
            presenter.Flow = this.Flow;

            presenter.MinSize = this.MinSize;
            presenter.MinFlow = this.MinFlow;

            presenter.SizePressure = this.SizePressure;
            presenter.FlowPressure = this.FlowPressure;

            presenter.Tip = this.Tip;
            presenter.IsStroke = this.IsStroke;

            presenter.Hardness = this.Hardness;

            // Texture
            presenter.Rotate = this.Rotate;
            presenter.Shape = this.Shape;
            presenter.RecolorShape = this.RecolorShape;

            presenter.GrainScale = this.GrainScale;
            presenter.Grain = this.Grain;
            presenter.RecolorGrain = this.RecolorGrain;

            presenter.BlendMode = this.BlendMode;

            // Mix
            presenter.Mix = this.Mix;
            presenter.Wet = this.Wet;
            presenter.Persistence = this.Persistence;
        }
    }
}