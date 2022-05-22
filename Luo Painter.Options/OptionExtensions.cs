using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Options
{
    public static class OptionExtensions
    {

        public static string GetThumbnail(this OptionType type) => $"ms-appx:///Luo Painter.Options/Thumbnails/{type}.jpg";
        public static string GetResource(this OptionType type) => $"ms-appx:///Luo Painter.Options/Icons/{type}Icon.xaml";
        public static ControlTemplate GetTemplate(this OptionType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;
        public static ControlTemplate GetTemplate(this OptionType type, out ResourceDictionary resource)
        {
            resource = new ResourceDictionary
            {
                Source = new Uri(type.GetResource())
            };
            return type.GetTemplate(resource);
        }

        public static bool AllowDrag(this OptionType type)
        {
            switch (type)
            {
                case OptionType.Transform:
                case OptionType.DisplacementLiquefaction:
                    return false;

                case OptionType.LuminanceToAlpha:
                    return false;

                default:
                    return true;
            }
        }

    }
}