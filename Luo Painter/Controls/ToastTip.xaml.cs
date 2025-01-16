using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ToastTip : UserControl
    {

        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        
        //@Construct
        public ToastTip()
        {
            this.InitializeComponent();
            this.HideStoryboard.Completed += (s, e) =>
            {
                base.Visibility = Visibility.Collapsed;
            };
            this.Timer.Tick += (s, e) =>
            {
                this.Timer.Stop();
                if (App.UISettings.AnimationsEnabled)
                {
                    this.HideStoryboard.Begin(); // Storyboard
                }
                else
                {
                    base.Visibility = Visibility.Collapsed;
                }
            };
        }

        public void Tip(string title, string subtitle)
        {
            this.TitleTextBlock.Text = title;
            this.SubtitleTextBlock.Text = subtitle;
            base.Visibility = Visibility.Visible;

            if (App.UISettings.AnimationsEnabled)
            {
                this.HideStoryboard.Stop(); // Storyboard
                this.ShowStoryboard.Begin(); // Storyboard
            }
            else
            {
                base.Visibility = Visibility.Visible;
            }

            this.Timer.Stop();
            this.Timer.Start();
        }

    }
}