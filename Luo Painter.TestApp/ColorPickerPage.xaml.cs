using Luo_Painter.HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed partial class ColorPickerPage : Page
    {
        readonly CanvasDevice CanvasDevice = new CanvasDevice();
        readonly float Dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

        readonly AlphaImageSource AlphaImageSource;
        readonly WheelImageSource WheelImageSource;

        IColorPicker ColorPicker => this.IndexGrid.Children[this.IndexGrid.Index] as IColorPicker;

        public ColorPickerPage()
        {
            this.InitializeComponent();
            this.AlphaImageSource = new AlphaImageSource(this.CanvasDevice, 320, 4, this.Dpi);
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(320), this.Dpi);

            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;

            this.AddButton.Click += (s, e) => this.ColorPicker.ZoomOut();
            this.RemoveButton.Click += (s, e) => this.ColorPicker.ZoomIn();

            this.LeftButton.Click += (s, e) => this.ColorPicker.Left();
            this.RightButton.Click += (s, e) => this.ColorPicker.Right();

            this.DownButton.Click += (s, e) => this.ColorPicker.Down();
            this.UpButton.Click += (s, e) => this.ColorPicker.Up();

            base.PointerWheelChanged += (s, e) =>
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this);

                float space = pointerPoint.Properties.MouseWheelDelta;

                if (space > 0)
                {
                    this.ColorPicker.ZoomOut();
                }
                else if (space < 0)
                {
                    this.ColorPicker.ZoomIn();
                }
            };
        }
        private void ColorChanged(object sender, Color e) => this.SolidColorBrush.Color = e;
        private void SurfaceContentsLost(object sender, object e)
        {
            this.AlphaImageSource.Redraw();
            this.WheelImageSource.Redraw();
        }
    }
}