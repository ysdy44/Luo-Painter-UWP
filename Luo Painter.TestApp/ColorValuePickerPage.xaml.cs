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
        readonly float Dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

        readonly AlphaImageSource AlphaImageSource;
        readonly WheelImageSource WheelImageSource;
        
        public ColorValuePickerPage()
        {            
            this.InitializeComponent();
            this.AlphaImageSource = new AlphaImageSource(this.CanvasDevice, 320, 4, this.Dpi);
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(320), this.Dpi);

            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;
        }
        private void ColorChanged(object sender, Color e) => this.SolidColorBrush.Color = e;
        private void SurfaceContentsLost(object sender, object e)
        {
            this.AlphaImageSource.Redraw();
            this.WheelImageSource.Redraw();
        }
    }
}