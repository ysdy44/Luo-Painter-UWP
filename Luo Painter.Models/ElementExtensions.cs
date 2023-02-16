using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Models
{
    public static class ElementExtensions
    {

        public static string GetResource(this ElementType type) => $"ms-appx:///Luo Painter.Models/Icons/{type}Icon.xaml";
        public static ControlTemplate GetTemplate(this ElementType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;
        public static ControlTemplate GetTemplate(this ElementType type, out ResourceDictionary resource)
        {
            resource = new ResourceDictionary
            {
                Source = new Uri(type.GetResource())
            };
            return type.GetTemplate(resource);
        }

    }
}