using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class ColorCommand : RelayCommand<Color> { }

    public sealed partial class ColorButton : Button, IInkParameter
    {
        //@Converter
        private Visibility ColorVisibilityConverter(int value) => value is 5 ? Visibility.Collapsed : Visibility.Visible;
        private Visibility PaletteVisibilityConverter(int value) => value is 5 ? Visibility.Visible : Visibility.Collapsed;
        private ColorSpectrumShape ColorSpectrumShapeConverter(int value) => value is 3 || value is 4 ? ColorSpectrumShape.Ring : ColorSpectrumShape.Box;
        private ColorSpectrumComponents ColorSpectrumComponentsConverter(int value)
        {
            switch (value)
            {
                case 0: return ColorSpectrumComponents.SaturationValue; // Hue
                case 1: return ColorSpectrumComponents.HueSaturation; // Saturation
                case 2: return ColorSpectrumComponents.HueValue; // Value

                case 3: return ColorSpectrumComponents.HueSaturation; // Saturation
                case 4: return ColorSpectrumComponents.HueValue; // Value

                default: return ColorSpectrumComponents.SaturationValue;
            }
        }

        //@Delegate
        public event TypedEventHandler<ColorPicker, ColorChangedEventArgs> ColorChanged;

        //@Content
        public Eyedropper Eyedropper { get; set; }
        public ClickEyedropper ClickEyedropper { get; set; }


        //@Task
        readonly object Locker = new object();
        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;
        BitmapLayer BitmapLayer { get; set; }

        Vector2 StartingPosition;
        Vector2 Position;
        float StartingPressure;
        float Pressure;

        ObservableCollection<Color> ObservableCollection { get; } = new ObservableCollection<Color>();
        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = System.TimeSpan.FromSeconds(1)
        };

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        public Color Color { get; private set; }
        public Vector4 ColorHdr { get; private set; }

        public string TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string path) => this.InkParameter.ConstructTexture(path);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;
        }

        #endregion


        Point StartingStraw;

        //@Construct
        public ColorButton()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.ConstructInk();

            this.ConstructColor();
            this.ConstructStraw();
            this.SetColorHdr(this.ColorPicker.Color);

            this.ColorPicker.Loaded += (s, e) =>
            {
                if (s is DependencyObject reference)
                {
                    DependencyObject grid = VisualTreeHelper.GetChild(reference, 0); // Grid
                    DependencyObject stackPanel = VisualTreeHelper.GetChild(grid, 0); // StackPanel

                    // 1. Slider
                    DependencyObject thirdDimensionSliderGrid = VisualTreeHelper.GetChild(stackPanel, 1); // Grid ThirdDimensionSliderGrid Margin 0,12,0,0
                    DependencyObject rectangle = VisualTreeHelper.GetChild(thirdDimensionSliderGrid, 0); // Rectangle Height 11

                    if (thirdDimensionSliderGrid is FrameworkElement thirdDimensionSliderGrid1)
                    {
                        thirdDimensionSliderGrid1.Margin = new Thickness(0);
                    }
                    if (rectangle is FrameworkElement rectangle1)
                    {
                        rectangle1.Height = 22;
                    }

                    // 2. ColorSpectrum
                    DependencyObject colorSpectrumGrid = VisualTreeHelper.GetChild(stackPanel, 0); // Grid ColorSpectrumGrid 
                    DependencyObject colorSpectrum = VisualTreeHelper.GetChild(colorSpectrumGrid, 0); // ColorSpectrum ColorSpectrum MaxWidth="336" MaxHeight="336" MinWidth="256" MinHeight="256" 

                    if (colorSpectrum is ColorSpectrum colorSpectrum1)
                    {
                        colorSpectrum1.MaxWidth = 1200;
                        colorSpectrum1.MaxHeight = 1200;
                    }
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Show(Color color)
        {
            this.ColorPicker.ColorChanged -= this.ColorChanged;
            this.ColorPicker.ColorChanged -= this.ColorPicker_ColorChanged;
            {
                this.ColorPicker.Color = color;
            }
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
        }
        public void ShowAt(Color color, FrameworkElement placementTarget)
        {
            this.ColorPicker.ColorChanged -= this.ColorChanged;
            this.ColorPicker.ColorChanged -= this.ColorPicker_ColorChanged;
            {
                this.ColorPicker.Color = color;
                base.Flyout.ShowAt(placementTarget);
            }
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
        }

    }
}