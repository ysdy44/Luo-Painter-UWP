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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    internal class ColorCommand : RelayCommand<Color> { }

    public sealed partial class ColorButton : Button, IInkParameter
    {
        //@Converter
        private Symbol ColorSpectrumShapeSymbolConverter(bool? value) => value == true ? Symbol.Target : Symbol.Stop;
        private ColorSpectrumShape ColorSpectrumShapeConverter(bool? value) => value == true ? ColorSpectrumShape.Ring : ColorSpectrumShape.Box;
        private ColorSpectrumComponents ColorSpectrumComponentsConverter(int value)
        {
            switch (value)
            {
                case 0: return ColorSpectrumComponents.SaturationValue;
                case 1: return ColorSpectrumComponents.HueSaturation;
                default: return ColorSpectrumComponents.HueValue;
            }
        }

        //@Delegate
        public event TypedEventHandler<ColorPicker, ColorChangedEventArgs> ColorChanged;

        //@Content
        public Eyedropper Eyedropper { get; set; }
        public ClickEyedropper ClickEyedropper { get; set; }

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

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
            this.ConstructColor();
            this.ConstructStraw();
            this.SetColorHdr(this.ColorPicker.Color);
        }

        /*
        private void ConstructColor()
        {
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.ColorChanged += (s, e) => this.SetColorHdr(e.NewColor);
            this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
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



            this.Timer.Tick += (s, e) =>
            {
                this.Timer.Stop();

                foreach (Color item in this.ObservableCollection)
                {
                    if (item == this.Color) return;
                }

                while (this.ObservableCollection.Count > 6)
                {
                    this.ObservableCollection.RemoveAt(0);
                }
                this.ObservableCollection.Add(this.Color);
            };
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Color item)
                {
                    this.ColorPicker.ColorChanged -= this.ColorPicker_ColorChanged;
                    this.ColorPicker.Color = item;
                    this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
                }
            };
        }


        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            this.Timer.Stop();
            this.Timer.Start();
        }

        private void SetColorHdr(Color color)
        {
            this.Color = color;
            this.ColorHdr = new Vector4(color.R, color.G, color.B, color.A) / 255f;
        }

        private void ConstructStraw()
        {
            base.ManipulationStarted += async (s, e) =>
            {
                if (this.Eyedropper is null) return;

                this.StartingStraw.X = Window.Current.Bounds.Width - 35;
                this.StartingStraw.Y = 25;

                bool result = await this.Eyedropper.RenderAsync(this.GetTarget());
            };
            base.ManipulationDelta += (s, e) =>
            {
                if (this.Eyedropper is null) return;

                switch (this.Eyedropper.Visibility)
                {
                    case Visibility.Collapsed:
                        if (e.Cumulative.Translation.ToVector2().LengthSquared() > 625)
                        {
                            Window.Current.CoreWindow.PointerCursor = null;
                            this.Eyedropper.Visibility = Visibility.Visible;
                        }
                        break;
                    case Visibility.Visible:
                        this.StartingStraw.X += e.Delta.Translation.X;
                        this.StartingStraw.Y += e.Delta.Translation.Y;
                        this.Eyedropper.Move(this.StartingStraw);
                        break;
                    default:
                        break;
                }
            };
            base.ManipulationCompleted += (s, e) =>
            {
                if (this.Eyedropper is null) return;

                switch (this.Eyedropper.Visibility)
                {
                    case Visibility.Visible:
                        Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                        this.Eyedropper.Visibility = Visibility.Collapsed;

                        this.ColorPicker.Color = this.Eyedropper.Color;
                        break;
                    default:
                        break;
                }
            };

            this.StrawButton.Click += async (s, e) =>
            {
                base.Flyout.Hide();
                if (this.ClickEyedropper is null) return;

                this.StartingStraw = this.StrawButton.TransformToVisual(Window.Current.Content).TransformPoint(new Point(20, 20));

                bool result = await this.ClickEyedropper.RenderAsync(this.GetTarget());
                if (result is false) return;

                this.ClickEyedropper.Move(this.StartingStraw);

                Window.Current.CoreWindow.PointerCursor = null;
                this.ClickEyedropper.Visibility = Visibility.Visible;

                this.ColorPicker.Color = await this.ClickEyedropper.OpenAsync();

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            };
        }
       */

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

        public UIElement GetTarget()
        {
            if (Window.Current.Content is FrameworkElement frame)
            {
                if (frame.Parent is FrameworkElement border)
                {
                    if (border.Parent is FrameworkElement rootScrollViewer)
                        return rootScrollViewer;
                    else
                        return border;
                }
                else
                    return frame;
            }
            else return Window.Current.Content;
        }

    }
}