using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton : Button, IInkParameter
    {

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
                    {
                        this.ColorPicker.Color = item;
                    }
                    this.ColorPicker.ColorChanged += this.ColorPicker_ColorChanged;
                }
            };
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

    }
}