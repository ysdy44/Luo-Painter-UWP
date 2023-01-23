using Luo_Painter.Brushes;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class PaintBrush : InkAttributes
    {
        public double Size2 { get => base.Size; set => base.Size = (float)value; }
        public double Opacity2 { get => base.Opacity; set => base.Opacity = (float)value; }

        public double Spacing2 { get => base.Spacing; set => base.Spacing = (float)value; }
        public double Flow2 { get => base.Flow; set => base.Flow = (float)value; }

        public string Path { get; set; } // = "Flash/00";
        public string Render => this.Path.GetThumbnailRender();
        public string Thumbnail => this.Path.GetThumbnail();

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();

        public string ImageSource => this.Tile == default ? InkExtensions.DeaultTile : this.Tile.GetTile();
        public int Tile2 { get => (int)this.Tile; set => this.Tile = (uint)value; }
        public uint Tile { get; set; } // = 0000000000;
    }

    public sealed partial class BrushGridView : XamlGridView
    {
        //@Delegate
        public event RoutedEventHandler Add { remove => this.AddButton.Click -= value; add => this.AddButton.Click += value; }

        //@Construct
        public BrushGridView()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) => base.SelectedIndex = 2;
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}