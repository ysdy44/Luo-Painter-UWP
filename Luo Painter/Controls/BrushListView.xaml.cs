using Luo_Painter.Brushes;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class PaintBrush : InkAttributes<double>
    {
        public string Path { get; set; } // = "Flash/00";
        public string Render => $"ms-appx:///Luo Painter.Brushes/Thumbnails/{this.Path}/Render.png";
        public string Thumbnail => $"ms-appx:///Luo Painter.Brushes/Thumbnails/{this.Path}/Thumbnail.png";

        public string Title { get; set; }
        public string Subtitle => ((int)this.Size).ToString();
    }

    public sealed partial class BrushListView : UserControl
    {
        //@Delegate
        public event EventHandler<InkAttributes<double>> ItemClick;
        public event RoutedEventHandler Add { remove => this.AddButton.Click -= value; add => this.AddButton.Click += value; }

        //@Construct
        public BrushListView()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is PaintBrush item)
                {
                    this.ItemClick?.Invoke(this, item);//Delegate
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}