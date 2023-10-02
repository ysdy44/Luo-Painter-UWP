using Luo_Painter.Brushes;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class TextureDialog : ContentDialog
    {

        //@Content
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