using Luo_Painter.Brushes;

namespace Luo_Painter.UI
{
    public sealed class PaintTexture
    {
        public string Path { get; set; } // = "Flash/00";
        public string Texture => this.Path.GetTexture();

        public int Width { get; set; }
        public int Height { get; set; }
        public int Step => System.Math.Max(this.Width, this.Height);
    }
}