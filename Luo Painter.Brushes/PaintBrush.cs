namespace Luo_Painter.Brushes
{
    public sealed class PaintBrush : PaintAttributes<double>
    {
        public string Path { get; set; } // = "Flash/00";
        public string Render => $"ms-appx:///Luo Painter.Brushes/Thumbnails/{this.Path}/Render.png";
        public string Thumbnail => $"ms-appx:///Luo Painter.Brushes/Thumbnails/{this.Path}/Thumbnail.png";
    
        public PaintTexture Shape { get; set; }
        public PaintTexture Grain { get; set; }

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();
    }
}