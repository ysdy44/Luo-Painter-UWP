using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Edits
{
    public static class EditExtensions
    {

        public static string GetResource(this EditType type) => $"ms-appx:///Luo Painter.Edits/Icons/{type}Icon.xaml";
        public static ControlTemplate GetTemplate(this EditType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;
        public static ControlTemplate GetTemplate(this EditType type, out ResourceDictionary resource)
        {
            resource = new ResourceDictionary
            {
                Source = new Uri(type.GetResource())
            };
            return type.GetTemplate(resource);
        }

        public static void GoToState(this Control control, bool isEnabled) => VisualStateManager.GoToState(control, isEnabled ? "Normal" : "Disabled", true);

    }
}