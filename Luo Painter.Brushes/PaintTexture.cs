namespace Luo_Painter.Brushes
{
    public sealed class PaintTexture
    {
        public string Path { get; set; } // = "Flash/00";
        public string Source => $@"Luo Painter.Brushes/{this.Path}/Source.png";
        public string Texture => $@"ms-appx:///Luo Painter.Brushes/{this.Path}/Texture.png";

        public int Width { get; set; }
        public int Height { get; set; }
        public int Step => System.Math.Max(this.Width, this.Height);
    }
}