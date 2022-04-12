using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

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