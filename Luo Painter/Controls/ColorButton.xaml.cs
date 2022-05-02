using Luo_Painter.Elements;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button
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
        public Color Color => this.ColorPicker.Color;

        Point StartingPosition;

        //@Construct
        public ColorButton()
        {
            this.InitializeComponent();
            this.ColorPicker.ColorChanged += this.ColorChanged;
            this.ColorPicker.Loaded += (s, e) =>
            {
                if (s is DependencyObject reference)
                {
                    DependencyObject grid = VisualTreeHelper.GetChild(reference, 0); // Grid
                    DependencyObject stackPanel = VisualTreeHelper.GetChild(grid, 0); // StackPanel
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
                }
            };

            base.Click += (s, e) =>
            {
                this.ColorFlyout.Placement = FlyoutPlacementMode.Left;
                this.ColorFlyout.ShowAt(this);
            };
            base.ManipulationStarted += async (s, e) =>
            {
                if (this.Eyedropper is null) return;

                this.StartingPosition.X = Window.Current.Bounds.Width - 35;
                this.StartingPosition.Y = 25;

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
                        this.StartingPosition.X += e.Delta.Translation.X;
                        this.StartingPosition.Y += e.Delta.Translation.Y;
                        this.Eyedropper.Move(this.StartingPosition);
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
                if (this.ClickEyedropper is null) return;

                this.StartingPosition = this.StrawButton.TransformToVisual(Window.Current.Content).TransformPoint(new Point(20, 20));
                this.ColorFlyout.Hide();

                bool result = await this.ClickEyedropper.RenderAsync(this.GetTarget());
                if (result is false) return;

                this.ClickEyedropper.Move(this.StartingPosition);

                Window.Current.CoreWindow.PointerCursor = null;
                this.ClickEyedropper.Visibility = Visibility.Visible;

                this.ColorPicker.Color = await this.ClickEyedropper.OpenAsync();

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Show(Color color)
        {
            this.ColorPicker.ColorChanged -= this.ColorChanged;
            {
                this.ColorPicker.Color = color;
            }
            this.ColorPicker.ColorChanged += this.ColorChanged;
        }
        public void ShowAt(Color color, FrameworkElement placementTarget, FlyoutPlacementMode placementMode = FlyoutPlacementMode.Top)
        {
            this.ColorPicker.ColorChanged -= this.ColorChanged;
            {
                this.ColorPicker.Color = color;
                this.ColorFlyout.Placement = placementMode;
                this.ColorFlyout.ShowAt(placementTarget);
            }
            this.ColorPicker.ColorChanged += this.ColorChanged;
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