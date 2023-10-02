using Luo_Painter.Brushes;
using System.Runtime.InteropServices;

namespace Luo_Painter.UI
{
    public sealed class PaintBrush : InkAttributes
    {
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
    }
}