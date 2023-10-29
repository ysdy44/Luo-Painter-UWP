using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Models
{
    public sealed class ElementResourceUri : Uri
    {
        public ElementResourceUri(ElementType type) : base($"ms-appx:///Luo Painter.Models/Icons/{type}Icon.xaml")
        {
        }
    }

    public static class ElementExtensions
    {
        public static ControlTemplate GetTemplate(this ElementType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;
    }
}