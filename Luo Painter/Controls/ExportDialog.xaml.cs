using Microsoft.Graphics.Canvas;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
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

        #region Converter

        //@Converter
        private Color FileColorConverter(CanvasBitmapFileFormat value)
        {
            switch (value)
            {
                case CanvasBitmapFileFormat.Jpeg: return Colors.SeaGreen;
                case CanvasBitmapFileFormat.Png: return Colors.DodgerBlue;
                case CanvasBitmapFileFormat.Bmp: return Colors.DarkTurquoise;
                case CanvasBitmapFileFormat.Gif: return Colors.MediumOrchid;
                case CanvasBitmapFileFormat.Tiff: return Colors.DeepPink;
                default: return Colors.Green;
            }
        }
        private string FileTitleConverter(CanvasBitmapFileFormat value)
        {
            switch (value)
            {
                case CanvasBitmapFileFormat.Jpeg: return "JPEG";
                case CanvasBitmapFileFormat.Png: return "PNG";
                case CanvasBitmapFileFormat.Bmp: return "BMP";
                case CanvasBitmapFileFormat.Gif: return "GIF";
                case CanvasBitmapFileFormat.Tiff: return "TIFF";
                default: return "JPEG";
            }
        }
        private string ChoicesConverter(CanvasBitmapFileFormat value)
        {
            switch (value)
            {
                case CanvasBitmapFileFormat.Jpeg: return ".jpeg";
                case CanvasBitmapFileFormat.Png: return ".png";
                case CanvasBitmapFileFormat.Bmp: return ".bmp";
                case CanvasBitmapFileFormat.Gif: return ".gif";
                case CanvasBitmapFileFormat.Tiff: return ".tiff";
                default: return ".jpeg";
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

        #endregion

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
        public string FileChoices => this.ChoicesConverter(this.FileFormat);

        #region DependencyProperty


        /// <summary> Gets or set the file format for <see cref="ExportDialog"/>. </summary>
        public CanvasBitmapFileFormat FileFormat
        {
            get => (CanvasBitmapFileFormat)base.GetValue(FileFormatProperty);
            set => base.SetValue(FileFormatProperty, value);
        }
        /// <summary> Identifies the <see cref = "ExportDialog.FileFormat" /> dependency property. </summary>
        public static readonly DependencyProperty FileFormatProperty = DependencyProperty.Register(nameof(FileFormat), typeof(CanvasBitmapFileFormat), typeof(ExportDialog), new PropertyMetadata(CanvasBitmapFileFormat.Jpeg));


        #endregion

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