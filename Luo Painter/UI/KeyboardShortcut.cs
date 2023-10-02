using Luo_Painter.Models;
using Windows.System;

namespace Luo_Painter.Controls
{
    public class KeyboardShortcut
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
}