using Luo_Painter.Brushes;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Controls
{
    internal sealed class PaintBrushGroupingList : List<PaintBrushGrouping>, IList<PaintBrushGrouping> { }
    internal class PaintBrushGrouping : List<PaintBrush>, IList<PaintBrush>, IGrouping<string, PaintBrush>
    {
        public string Key { set; get; }
    }
    internal sealed class PaintBrush : InkAttributes
    {
        public double Size2 { get => base.Size; set => base.Size = (float)value; }
        public double Opacity2 { get => base.Opacity; set => base.Opacity = (float)value; }

        public double Spacing2 { get => base.Spacing; set => base.Spacing = (float)value; }
        public double Flow2 { get => base.Flow; set => base.Flow = (float)value; }

        public double MinSize2 { get => base.MinSize; set => base.MinSize = (float)value; }
        public double MinFlow2 { get => base.MinFlow; set => base.MinFlow = (float)value; }

        public string ImageSource => base.Tile == default ? InkExtensions.DeaultTile : base.Tile.GetTile();
        public int Tile2 { get => (int)base.Tile; set => base.Tile = (uint)value; }
    }

    public sealed partial class BrushGridView : XamlGridView
    {

        //@Construct
        public BrushGridView()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => base.SelectedIndex = 2;
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}