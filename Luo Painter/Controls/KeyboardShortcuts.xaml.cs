using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Controls
{
    internal struct KeyboardShortcut
    {
        public readonly string Key;
        public readonly string Value;
        public KeyboardShortcut(KeyboardAccelerator key)
        {
            this.Key = key.ToString();
            switch (key.Modifiers)
            {
                case VirtualKeyModifiers.None: this.Value = $"{key.Key}"; break;
                case VirtualKeyModifiers.Control: this.Value = $"Ctrl + {key.Key}"; break;
                case VirtualKeyModifiers.Menu: this.Value = $"Alt + {key.Key}"; break;
                case VirtualKeyModifiers.Shift: this.Value = $"Shift + {key.Key}"; break;
                case VirtualKeyModifiers.Windows: this.Value = $"Win + {key.Key}"; break;
                default: this.Value = $"{key.Key}"; break;
            }
        }
    }

    public sealed partial class KeyboardShortcuts : UserControl
    {

        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };

        public object ItemsSource { set => this.ItemsControl.ItemsSource = value; }

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