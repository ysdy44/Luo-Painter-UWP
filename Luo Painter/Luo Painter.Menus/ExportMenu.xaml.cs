using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Menus
{
    public sealed partial class ExportMenu : Expander
    {

        //@Delegate
        public event RoutedEventHandler ExportClick
        {
            remove => this.Button.Click -= value;
            add => this.Button.Click += value;
        }

        //@Content
        public bool IsAllLayers => this.ComboBox.SelectedIndex is 0 is false;
        public int DPI
        {
            get
            {
                switch (this.DPIComboBox.SelectedIndex)
                {
                    case 0: return 72;
                    case 1: return 96;
                    case 2: return 144;
                    case 3: return 192;
                    case 4: return 300;
                    case 5: return 400;
                    default: return 96;
                }
            }
        }
        public CanvasBitmapFileFormat FileFormat
        {
            get
            {
                switch (this.FormatComboBox.SelectedIndex)
                {
                    case 0: return CanvasBitmapFileFormat.Jpeg;
                    case 1: return CanvasBitmapFileFormat.Png;
                    case 2: return CanvasBitmapFileFormat.Bmp;
                    case 3: return CanvasBitmapFileFormat.Gif;
                    case 4: return CanvasBitmapFileFormat.Tiff;
                    default: return CanvasBitmapFileFormat.Jpeg;
                }
            }
        }
        public string FileChoices
        {
            get
            {
                switch (this.FormatComboBox.SelectedIndex)
                {
                    case 0: return ".jpeg";
                    case 1: return ".png";
                    case 2: return ".bmp";
                    case 3: return ".gif";
                    case 4: return ".tiff";
                    default: return ".jpeg";
                }
            }
        }

        //@Construct
        public ExportMenu()
        {
            this.InitializeComponent();
            this.Canvas.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.FlipView.Width = e.NewSize.Width;
                this.FlipView.Height = e.NewSize.Height;
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}