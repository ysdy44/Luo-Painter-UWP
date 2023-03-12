using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Luo_Painter.Models;

namespace Luo_Painter.Controls
{
    internal class KeyboardShortcut
    {
        public VirtualKeyModifiers Modifiers { get; set; }
        public VirtualKey Key { get; set; }
        public OptionType CommandParameter { get; set; }
        public override string ToString()
        {
            switch (this.Modifiers)
            {
                case VirtualKeyModifiers.None: return $"{this.Key}";
                case VirtualKeyModifiers.Control: return $"Ctrl + {this.Key}";
                case VirtualKeyModifiers.Menu: return $"Alt + {this.Key}";
                case VirtualKeyModifiers.Shift: return $"Shift + {this.Key}";
                case VirtualKeyModifiers.Windows: return $"Win + {this.Key}";
                default: return $"{this.Key}";
            }
        }
    }

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