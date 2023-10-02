using Luo_Painter.Brushes;
using System.Collections.Generic;
using System.Linq;

namespace Luo_Painter.Controls
{
    public enum InkGroupingType
    {
        Erase = 8,
        Paint = 16,
        Fx = 32,
        Others = 64,

        Flash = 1 | Others,
        Splodge = 2 | Others,
        Scratch = 3 | Others,
    }
    internal sealed class InkGroupingList : List<InkGrouping>, IList<InkGrouping> { }
    internal class InkGrouping : List<InkAttributes>, IList<InkAttributes>, IGrouping<InkGroupingType, InkAttributes>
    {
        public InkGroupingType Key { set; get; }
        public string KeyString => App.Resource.GetString($"Brush_{(this.Key.HasFlag(InkGroupingType.Others) ? InkGroupingType.Others : this.Key)}");
    }

    internal class BrushBase : InkAttributes
    {
        public double Size2 { get => base.Size; set => base.Size = (float)value; }
        public double Opacity2 { get => base.Opacity; set => base.Opacity = (float)value; }

        public double Spacing2 { get => base.Spacing; set => base.Spacing = (float)value; }
        public double Flow2 { get => base.Flow; set => base.Flow = (float)value; }

        public double MinSize2 { get => base.MinSize; set => base.MinSize = (float)value; }
        public double MinFlow2 { get => base.MinFlow; set => base.MinFlow = (float)value; }

        public double GrainScale2 { get => base.GrainScale; set => base.GrainScale = (float)value; }
    }
    internal sealed class PaintBrush : BrushBase
    {
        public uint Tile { get; set; }
        public string ImageSource => this.Tile == default ? BrushExtensions.DeaultTile : this.Tile.GetTile();
        public int Tile2 { get => (int)this.Tile; set => this.Tile = (uint)value; }
    }

    public sealed partial class BrushGridView : XamlGridView
    {

        //@Construct
        public BrushGridView()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => base.SelectedIndex = 2;
        }

    }
}