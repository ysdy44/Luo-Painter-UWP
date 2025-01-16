using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.UI
{
    public sealed class PaintBrush
    {
        public InkType Type { get; set; } = InkType.General;

        // Property
        public double Size2 { get; set; }
        public double Opacity2 { get; set; }

        public double Spacing2 { get; set; }
        public double Flow2 { get; set; }

        public double MinSize2 { get; set; }
        public double MinFlow2 { get; set; }

        public double GrainScale2 { get; set; }

        public uint Tile => (uint)this.Tile2;
        public string ImageSource => this.Tile == default ? BrushExtensions.DeaultTile : this.Tile.GetTile();
        public int Tile2 { get; set; }

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
            presenter.Size = (float)this.Size2;
            presenter.Opacity = (float)this.Opacity2;
            presenter.Spacing = (float)this.Spacing2;
            presenter.Flow = (float)this.Flow2;

            presenter.MinSize = (float)this.MinSize2;
            presenter.MinFlow = (float)this.MinFlow2;

            presenter.SizePressure = this.SizePressure;
            presenter.FlowPressure = this.FlowPressure;

            presenter.Tip = this.Tip;
            presenter.IsStroke = this.IsStroke;

            presenter.Hardness = this.Hardness;

            // Texture
            presenter.Rotate = this.Rotate;
            presenter.Shape = this.Shape;
            presenter.RecolorShape = this.RecolorShape;

            presenter.GrainScale = (float)this.GrainScale2;
            presenter.Grain = this.Grain;
            presenter.RecolorGrain = this.RecolorGrain;

            presenter.BlendMode = this.BlendMode;

            // Mix
            presenter.Mix = (float)this.Mix2;
            presenter.Wet = (float)this.Wet2;
            presenter.Persistence = (float)this.Persistence2;
        }
    }
}