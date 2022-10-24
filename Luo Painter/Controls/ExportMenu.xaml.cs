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
        private Color FileColorConverter(int value)
        {
            switch (value)
            {
                case 0: return Colors.SeaGreen;
                case 1: return Colors.DodgerBlue;
                case 2: return Colors.DarkTurquoise;
                case 3: return Colors.MediumOrchid;
                case 4: return Colors.DeepPink;
                default: return Colors.Green;
            }
        }
        private string FileTitleConverter(int value)
        {
            switch (value)
            {
                case 0: return "JPEG";
                case 1: return "PNG";
                case 2: return "BMP";
                case 3: return "GIF";
                case 4: return "TIFF";
                default: return "JPEG";
            }
        }
        private string FileChoicesConverter(int value)
        {
            switch (value)
            {
                case 0: return ".jpeg";
                case 1: return ".png";
                case 2: return ".bmp";
                case 3: return ".gif";
                case 4: return ".tiff";
                default: return ".jpeg";
            }
        }
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
        public bool IsOpenFileExplorer => this.CheckBox.IsChecked is true;
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