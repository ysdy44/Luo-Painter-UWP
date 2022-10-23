using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class PaintTexture
    {
        public string Path { get; set; } // = "Flash/00";
        public string Source => $@"Luo Painter.Brushes/Textures/{this.Path}/Source.png";
        public string Texture => $@"ms-appx:///Luo Painter.Brushes/Textures/{this.Path}/Texture.png";

        public int Width { get; set; }
        public int Height { get; set; }
        public int Step => System.Math.Max(this.Width, this.Height);
    }

    internal sealed class PaintTextureList : List<PaintTexture>
    {
        public int IndexOf(string path)
        {
            if (string.IsNullOrEmpty(path)) return -1;

            for (int i = 0; i < base.Count; i++)
            {
                PaintTexture item = base[i];
                if (item.Path == path)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public sealed partial class TextureDialog : ContentDialog
    {

        public string SelectedItem => (this.GridView.SelectedItem is PaintTexture item) ? item.Path : null;

        //@Construct
        public TextureDialog()
        {
            this.InitializeComponent();
        }

        public void Construct(string path)
        {
            this.GridView.SelectedIndex = this.Collection.IndexOf(path);
        }

    }
}