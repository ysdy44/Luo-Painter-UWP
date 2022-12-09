using Luo_Painter.HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Display;
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

        public ColorPickerPage()
        {
            this.InitializeComponent();
            this.AlphaImageSource = new AlphaImageSource(this.CanvasDevice, 320, 4, this.Dpi);
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(320), this.Dpi);

            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;

            this.AddButton.Click += (s, e) => this.TricolorPicker.ZoomOut();
            this.RemoveButton.Click += (s, e) => this.TricolorPicker.ZoomIn();

            this.LeftButton.Click += (s, e) => this.TricolorPicker.Left();
            this.RightButton.Click += (s, e) => this.TricolorPicker.Right();

            this.DownButton.Click += (s, e) => this.TricolorPicker.Down();
            this.UpButton.Click += (s, e) => this.TricolorPicker.Up();

            this.TricolorPicker.ColorChanged += (s, e) => this.SolidColorBrush.Color = e;
            this.TricolorPicker.PointerWheelChanged += (s, e) =>
            {
                PointerPoint pointerPoint = e.GetCurrentPoint(this.TricolorPicker);

                float space = pointerPoint.Properties.MouseWheelDelta;

                if (space > 0)
                {
                    this.TricolorPicker.ZoomOut();
                }
                else if (space < 0)
                {
                    this.TricolorPicker.ZoomIn();
                }
            };
        }
        private void SurfaceContentsLost(object sender, object e)
        {
            this.AlphaImageSource.Redraw();
            this.WheelImageSource.Redraw();
        }
    }
}