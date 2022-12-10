using Luo_Painter.HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed partial class ColorValuePickerPage : Page
    {
        readonly CanvasDevice CanvasDevice = new CanvasDevice();
        readonly OpacityImageSource OpacityImageSource;
        readonly AlphaImageSource AlphaImageSource;
        readonly WheelImageSource WheelImageSource;

        public ColorValuePickerPage()
        {
            this.InitializeComponent();
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;
            this.OpacityImageSource = new OpacityImageSource(this.CanvasDevice, 90, 15, dpi);
            this.AlphaImageSource = new AlphaImageSource(this.CanvasDevice, 320, 4, dpi);
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(320), dpi);

            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;
            
            this.RGBPicker.Recolor(Colors.DodgerBlue);
            this.HSVPicker.Recolor(Colors.DodgerBlue);
            this.HexPicker.Recolor(Colors.DodgerBlue);
            this.RGBPicker.ColorChanged += (s, e) => this.SolidColorBrush.Color = e;
            this.HSVPicker.ColorChanged += (s, e) => this.SolidColorBrush.Color = e;
            this.HexPicker.ColorChanged += (s, e) => this.SolidColorBrush.Color = e;
        }
        private void SurfaceContentsLost(object sender, object e)
        {
            this.OpacityImageSource.Redraw();
            this.AlphaImageSource.Redraw();
            this.WheelImageSource.Redraw();
        }
    }
}