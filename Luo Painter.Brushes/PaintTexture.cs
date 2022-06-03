namespace Luo_Painter.Brushes
{
    public sealed class PaintTexture
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Source { get; set; }
        public string Texture { get; set; }
        public int Step => System.Math.Max(this.Width, this.Height);
    }
}