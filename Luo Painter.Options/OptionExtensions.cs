using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Options
{
    public static class OptionExtensions
    {

        public static bool IsItemClickEnabled(this OptionType type) => type.HasFlag(OptionType.IsItemClickEnabled);
        public static bool ExistIcon(this OptionType type) => type.HasFlag(OptionType.ExistIcon);
        public static bool ExistThumbnail(this OptionType type) => type.HasFlag(OptionType.ExistThumbnail);
        public static bool AllowDrag(this OptionType type) => type.HasFlag(OptionType.AllowDrag);
        public static bool HasPreview(this OptionType type) => type.HasFlag(OptionType.HasPreview);
        public static bool HasDifference(this OptionType type) => type.HasFlag(OptionType.HasDifference);
        public static bool WithState(this OptionType type) => type.HasFlag(OptionType.WithState);
        public static bool WithTransform(this OptionType type) => type.HasFlag(OptionType.WithTransform);
        public static void GoToState(this Control control, bool isEnabled) => VisualStateManager.GoToState(control, isEnabled ? "Normal" : "Disabled", true);

        public static bool IsEffect(this OptionType type) => type.HasFlag(OptionType.Effect);
        public static bool IsEdit(this OptionType type) => type.HasFlag(OptionType.Marquees);

        public static bool IsMarquee(this OptionType type) => type.HasFlag(OptionType.Marquee);
        public static bool IsSelection(this OptionType type) => type.HasFlag(OptionType.Selection);
        public static bool IsPaint(this OptionType type) => type.HasFlag(OptionType.Paint);
        public static bool IsGeometry(this OptionType type) => type.HasFlag(OptionType.Geometry);
        public static OptionType ToGeometryTransform(this OptionType type) => type.IsGeometry() ? (type | OptionType.HasPreview | OptionType.AllowDrag) : type;

        public static string GetThumbnail(this OptionType type) => type.ExistThumbnail() ? $"ms-appx:///Luo Painter.Options/Thumbnails/{type}.jpg" : throw new NullReferenceException($"The {type} no Tumbnail.");
        public static string GetResource(this OptionType type) => type.ExistIcon() ? $"ms-appx:///Luo Painter.Options/Icons/{type}Icon.xaml" : throw new NullReferenceException($"The {type} no Icon.");
        public static ControlTemplate GetTemplate(this OptionType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;
        public static ControlTemplate GetTemplate(this OptionType type, out ResourceDictionary resource)
        {
            resource = new ResourceDictionary
            {
                Source = new Uri(type.GetResource())
            };
            return type.GetTemplate(resource);
        }

    }
}