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

        public static bool HasPreview(this OptionType type)
        {
            switch (type)
            {
                case OptionType.None:
                case OptionType.Gray:
                case OptionType.Invert:
                    return false;

                case OptionType.Fog:
                case OptionType.Sepia:
                case OptionType.Posterize:
                    return false;

                default:
                    return true;
            }
        }

        public static bool HasDifference(this OptionType type)
        {
            switch (type)
            {
                case OptionType.Transform:
                case OptionType.RippleEffect:
                    return true;

                default:
                    return false;
            }
        }

        public static bool HasIcon(this OptionType type)
        {
            switch (type)
            {
                case OptionType.GradientMapping:
                case OptionType.RippleEffect:
                    return true;

                case OptionType.Exposure:
                case OptionType.Brightness:
                case OptionType.Saturation:
                case OptionType.HueRotation:
                case OptionType.Contrast:
                case OptionType.Temperature:
                case OptionType.HighlightsAndShadows:
                    return true;

                default:
                    return false;
            }
        }

    }
}