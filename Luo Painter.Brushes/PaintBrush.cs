namespace Luo_Painter.Brushes
{
    public sealed class PaintBrush
    {
        public InkType Type { get; set; } = InkType.General;

        public double Size { get; set; } = 12;
        public double Opacity { get; set; } = 1;
        public double Spacing { get; set; } = 0.25;
        public double Flow { get; set; } = 1;

        public BrushEdgeHardness Hardness { get; set; } = BrushEdgeHardness.None;

        public bool Rotate { get; set; }
        public int Step => (this.Grain is null) ? 0 : this.Grain.Step;

        public string Render { get; set; }
        public string Thumbnail { get; set; }
        public PaintTexture Shape { get; set; }
        public PaintTexture Grain { get; set; }

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();
    }
}