namespace Luo_Painter.Brushes
{
    public sealed class PaintBrush
    {
        public double Size { get; set; } = 12;
        public double Opacity { get; set; } = 1;
        public double Space { get; set; } = 0.25;
        public BrushEdgeHardness Hardness { get; set; } = BrushEdgeHardness.None;

        public string Render { get; set; }
        public string Thumbnail { get; set; }
        public string Source { get; set; }

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();
    }
}