using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        Point StartingEyedropper;
        readonly DispatcherTimer TipTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        private void ConstructTip()
        {
            this.HideTipStoryboard.Completed += (s, e) =>
            {
                this.TipBorder.Visibility = Visibility.Collapsed;
            };
            this.TipTimer.Tick += (s, e) =>
            {
                this.TipTimer.Stop();
                this.HideTipStoryboard.Begin(); // Storyboard
            };
        }

        public void Tip(string title, string subtitle)
        {
            this.TipTitleTextBlock.Text = title;
            this.TipSubtitleTextBlock.Text = subtitle;
            this.TipBorder.Visibility = Visibility.Visible;

            this.HideTipStoryboard.Stop(); // Storyboard
            this.ShowTipStoryboard.Begin(); // Storyboard

            this.TipTimer.Stop();
            this.TipTimer.Start();
        }


        private void ConstructColor()
        {
            this.StrawButton.Click += async (s, e) =>
            {
                this.StartingEyedropper = this.StrawButton.TransformToVisual(Window.Current.Content).TransformPoint(new Point(20, 20));
                this.ColorFlyout.Hide();

                bool result = await this.ClickEyedropper.RenderAsync(this.GetEyedropperTarget());
                if (result is false) return;

                this.ClickEyedropper.Move(this.StartingEyedropper);

                Window.Current.CoreWindow.PointerCursor = null;
                this.ClickEyedropper.Visibility = Visibility.Visible;

                this.ColorPicker.Color = await this.ClickEyedropper.OpenAsync();

                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.ClickEyedropper.Visibility = Visibility.Collapsed;
            };


            this.ColorButton.ManipulationStarted += async (s, e) =>
            {
                this.StartingEyedropper.X = base.ActualWidth - 35;
                this.StartingEyedropper.Y = 25;

                bool result = await this.Eyedropper.RenderAsync(this.GetEyedropperTarget());
                if (result is false) return;

                Window.Current.CoreWindow.PointerCursor = null;
                this.Eyedropper.Visibility = Visibility.Visible;
            };
            this.ColorButton.ManipulationDelta += (s, e) =>
            {
                this.StartingEyedropper.X += e.Delta.Translation.X;
                this.StartingEyedropper.Y += e.Delta.Translation.Y;
                this.Eyedropper.Move(this.StartingEyedropper);
            };
            this.ColorButton.ManipulationCompleted += (s, e) =>
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                this.Eyedropper.Visibility = Visibility.Collapsed;

                this.ColorPicker.Color = this.Eyedropper.Color;
            };
        }

        private UIElement GetEyedropperTarget()
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

        private void ConstructColorShape()
        {
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
        }

    }
}