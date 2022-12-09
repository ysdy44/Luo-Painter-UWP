using Luo_Painter.HSVColorPickers;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Display;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed partial class ColorHarmonyPickerPage : Page
    {
        //@Converter
        private Visibility Visibility1Converter(HarmonyMode value) => value.HasFlag(HarmonyMode.HasPoint1) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Visibility2Converter(HarmonyMode value) => value.HasFlag(HarmonyMode.HasPoint2) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Visibility3Converter(HarmonyMode value) => value.HasFlag(HarmonyMode.HasPoint3) ? Visibility.Visible : Visibility.Collapsed;
        private HarmonyMode ModeConverter(int value)
        {
            switch (value)
            {
                case 0: return HarmonyMode.None;
                case 1: return HarmonyMode.Complementary;
                case 2: return HarmonyMode.SplitComplementary;
                case 3: return HarmonyMode.Analogous;
                case 4: return HarmonyMode.Triadic;
                case 5: return HarmonyMode.Tetradic;
                default: return default;
            }
        }

        readonly CanvasDevice CanvasDevice = new CanvasDevice();
        readonly float Dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

        readonly AlphaImageSource AlphaImageSource;
        readonly WheelImageSource WheelImageSource;

        IColorPicker ColorPicker => this.HarmonyPicker;

        #region DependencyProperty

        /// <summary> Gets or set the mode for <see cref="ColorHarmonyPickerPage"/>. </summary>
        public HarmonyMode Mode
        {
            get => (HarmonyMode)base.GetValue(ModeProperty);
            set => base.SetValue(ModeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ColorHarmonyPickerPage.Mode" /> dependency property. </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(HarmonyMode), typeof(ColorHarmonyPickerPage), new PropertyMetadata(HarmonyMode.None, (sender, e) =>
        {
            ColorHarmonyPickerPage control = (ColorHarmonyPickerPage)sender;

            if (e.NewValue is HarmonyMode value)
            {
                control.HarmonyPicker.Remode(value);
            }
        }));

        #endregion

        public ColorHarmonyPickerPage()
        {
            this.InitializeComponent();
            this.AlphaImageSource = new AlphaImageSource(this.CanvasDevice, 320, 4, this.Dpi);
            this.WheelImageSource = new WheelImageSource(this.CanvasDevice, new CircleTemplateSettingsF(320), this.Dpi);

            base.Unloaded += (s, e) => CompositionTarget.SurfaceContentsLost -= this.SurfaceContentsLost;
            base.Loaded += (s, e) => CompositionTarget.SurfaceContentsLost += this.SurfaceContentsLost;

            this.HarmonyPicker.ColorChanged += (s, e) => this.SolidColorBrush.Color =  this.EllipseSolidColorBrush.Color = e;
            this.HarmonyPicker.Color1Changed += (s, e) => this.Ellipse1SolidColorBrush.Color = e;
            this.HarmonyPicker.Color2Changed += (s, e) => this.Ellipse2SolidColorBrush.Color = e;
            this.HarmonyPicker.Color3Changed += (s, e) => this.Ellipse3SolidColorBrush.Color = e;

            this.ListView.SelectionChanged += (s, e) => this.Mode = this.ModeConverter(this.ListView.SelectedIndex);

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
        private void SurfaceContentsLost(object sender, object e)
        {
            this.AlphaImageSource.Redraw();
            this.WheelImageSource.Redraw();
        }
    }
}