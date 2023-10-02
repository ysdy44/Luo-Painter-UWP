using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Models;

namespace Luo_Painter.Controls
{
    public sealed partial class KeyboardShortcuts : ItemsControl
    {

        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        //@Construct
        public KeyboardShortcuts()
        {
            this.InitializeComponent();
            this.HideStoryboard.Completed += (s, e) =>
            {
                base.Visibility = Visibility.Collapsed;
            };
            this.Timer.Tick += (s, e) =>
            {
                this.Timer.Stop();
                this.HideStoryboard.Begin(); // Storyboard
            };
        }

        public void Tip()
        {
            base.Visibility = Visibility.Visible;

            this.HideStoryboard.Stop(); // Storyboard
            this.ShowStoryboard.Begin(); // Storyboard

            this.Timer.Stop();
            this.Timer.Start();
        }

    }
}