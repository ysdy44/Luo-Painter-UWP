using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorPicker2 : UserControl
    {
        //@Converter
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
        public event TypedEventHandler<ColorPicker, ColorChangedEventArgs> ColorChanged { remove => this.ColorPicker.ColorChanged -= value; add => this.ColorPicker.ColorChanged += value; }

        //@Content
        public Color Color { get => this.ColorPicker.Color; set => this.ColorPicker.Color = value; }

        //@Construct
        public ColorPicker2()
        {
            this.InitializeComponent();
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

    }
}