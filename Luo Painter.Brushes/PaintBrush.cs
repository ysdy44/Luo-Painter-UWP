namespace Luo_Painter.Brushes
{
    public sealed class PaintBrush
    {
        public double Size { get; set; } = 12;
        public double Opacity { get; set; } = 1;
        public double Spacing { get; set; } = 0.25;
        public double Flow { get; set; } = 1;
      
        public BrushEdgeHardness Hardness { get; set; } = BrushEdgeHardness.None;

        public bool Rotate { get; set; }
        public int Step => (this.Pattern is null) ? 0 : this.Pattern.Step;

        public string Render { get; set; }
        public string Thumbnail { get; set; }
        public PaintTexture Mask { get; set; }
        public PaintTexture Pattern { get; set; }

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();
    }
}