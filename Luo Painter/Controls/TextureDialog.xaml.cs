using Luo_Painter.Brushes;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class PaintTextureList : List<PaintTexture>
    {
        public int IndexOf(string texture)
        {
            if (string.IsNullOrEmpty(texture)) return -1;

            for (int i = 0; i < base.Count; i++)
            {
                PaintTexture item = base[i];
                if (item.Texture == texture)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public sealed partial class TextureDialog : ContentDialog
    {

        public object SelectedItem => this.GridView.SelectedItem;

        //@Construct
        public TextureDialog()
        {
            this.InitializeComponent();
        }

        public void Construct(string texture)
        {
            this.GridView.SelectedIndex = this.Collection.IndexOf(texture);
        }

    }
}