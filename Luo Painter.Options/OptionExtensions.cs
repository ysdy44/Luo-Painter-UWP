using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Models
{
    public static class OptionExtensions
    {

        public static bool IsItemClickEnabled(this OptionType type) => type.HasFlag(OptionType.IsItemClickEnabled);
        public static bool ExistIcon(this OptionType type) => type.HasFlag(OptionType.ExistIcon);
        public static bool ExistThumbnail(this OptionType type) => type.HasFlag(OptionType.ExistThumbnail);
        public static bool HasMenu(this OptionType type) => type.HasFlag(OptionType.HasMenu);
        public static bool HasPreview(this OptionType type) => type.HasFlag(OptionType.HasPreview);
        public static bool HasDifference(this OptionType type) => type.HasFlag(OptionType.HasDifference);
        public static bool WithState(this OptionType type) => type.HasFlag(OptionType.WithState);
        public static bool WithTransform(this OptionType type) => type.HasFlag(OptionType.WithTransform);
        public static void GoToState(this MenuFlyoutItemBase control, bool isEnabled)
        {
            if (control.IsEnabled == isEnabled) return;
            control.IsEnabled = isEnabled;

            if (control.Tag is Control icon)
            {
                VisualStateManager.GoToState(icon, isEnabled ? "Normal" : "Disabled", true);
            }
        }

        public static bool IsEffect(this OptionType type) => type.HasFlag(OptionType.Effect);
        public static bool IsMarquees(this OptionType type) => type.HasFlag(OptionType.Marquees);

        public static bool IsMarquee(this OptionType type) => type.HasFlag(OptionType.Marquee);
        public static bool IsSelection(this OptionType type) => type.HasFlag(OptionType.Selection);
        public static bool IsPaint(this OptionType type) => type.HasFlag(OptionType.Paint);
        public static bool IsGeometry(this OptionType type) => type.HasFlag(OptionType.Geometry);
        public static bool IsPattern(this OptionType type) => type.HasFlag(OptionType.Pattern);
  
        public static OptionType ToGeometryTransform(this OptionType type) => type.IsGeometry() ? (type | OptionType.WithTransform | OptionType.HasPreview) : type;
        public static OptionType ToPatternTransform(this OptionType type) => type.IsPattern() ? (type | OptionType.WithTransform | OptionType.HasPreview) : type;

        //@Resource
        public static string GetThumbnail(this OptionType type) => type.ExistThumbnail() ? $"ms-appx:///Luo Painter.Options/Thumbnails/{type}.jpg" : throw new NullReferenceException($"The {type} no Tumbnail.");
        public static string GetResource(this OptionType type) => type.ExistIcon() ? $"ms-appx:///Luo Painter.Options/Icons/{type}Icon.xaml" : throw new NullReferenceException($"The {type} no Icon.");
        public static ControlTemplate GetTemplate(this OptionType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;

    }
}