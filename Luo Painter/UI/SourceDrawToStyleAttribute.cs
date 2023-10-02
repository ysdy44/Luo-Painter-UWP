using System;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.UI
{
    internal sealed class SourceDrawToStyleAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public SourceDrawToStyleAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(DrawPage)} to {typeof(StylePage)}, NavigationMode is {this.NavigationMode}";
    }
}