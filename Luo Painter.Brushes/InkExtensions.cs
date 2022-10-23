using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Luo_Painter.Brushes
{
    public static class InkExtensions
    {

        public static string GetResource(this InkType type) => $"ms-appx:///Luo Painter.Brushes/Icons/{type}Icon.xaml";
        public static ControlTemplate GetTemplate(this InkType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;

    }
}