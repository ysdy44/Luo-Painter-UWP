namespace Luo_Painter.Brushes
{
    public sealed class PaintBrush : PaintAttributes<double>
    {
        public string Render { get; set; }
        public string Thumbnail { get; set; }
        public PaintTexture Shape { get; set; }
        public PaintTexture Grain { get; set; }

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();
    }
}