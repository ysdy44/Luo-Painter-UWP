using Luo_Painter.Options;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
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
            this.ColorButton.ColorChanged += (s, e) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingColorChanged(e.NewColor);
                        break;
                    default:
                        break;
                }
            };
        }

    }
}