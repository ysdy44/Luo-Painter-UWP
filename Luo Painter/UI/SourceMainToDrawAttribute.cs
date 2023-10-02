using Luo_Painter.Models;
using System;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter
{
    internal sealed class SourceMainToDrawAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public SourceMainToDrawAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(MainPage)} to {typeof(DrawPage)}, Parameter is {typeof(ProjectParameter)}, NavigationMode is {this.NavigationMode}";
    }
}