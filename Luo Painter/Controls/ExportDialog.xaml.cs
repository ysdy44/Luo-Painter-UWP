using Luo_Painter.Blends;
using Microsoft.Graphics.Canvas;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public enum ExportMode
    {
        None,
        All,
        Current,
    }

    public sealed partial class ExportDialog : ContentDialog
    {

        //@Converter
        private Color FileColorConverter(int value) => this.FileFormatConverter(value).ToColor();
        private string FileTitleConverter(int value) => this.FileFormatConverter(value).GetTitle();
        private string FileChoicesConverter(int value) => this.FileFormatConverter(value).GetChoices();
        private CanvasBitmapFileFormat FileFormatConverter(int value)
        {
            switch (value)
            {
                case 0: return CanvasBitmapFileFormat.Jpeg;
                case 1: return CanvasBitmapFileFormat.Png;
                case 2: return CanvasBitmapFileFormat.Bmp;
                case 3: return CanvasBitmapFileFormat.Gif;
                case 4: return CanvasBitmapFileFormat.Tiff;
                default: return CanvasBitmapFileFormat.Jpeg;
            }
        }
        private int DPIConverter(int value)
        {
            switch (value)
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

        //@Content
        public ExportMode Mode
        {
            get
            {
                switch (this.ComboBox.SelectedIndex)
                {
                    case 0: return ExportMode.None;
                    case 1: return ExportMode.All;
                    case 2: return ExportMode.Current;
                    default: return ExportMode.None;
                }
            }
        }

        public int DPI => this.DPIConverter(this.DPIComboBox.SelectedIndex);
        public CanvasBitmapFileFormat FileFormat => this.FileFormatConverter(this.FormatComboBox.SelectedIndex);
        public string FileChoices => this.FileChoicesConverter(this.FormatComboBox.SelectedIndex);

        //@Construct
        public ExportDialog()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}