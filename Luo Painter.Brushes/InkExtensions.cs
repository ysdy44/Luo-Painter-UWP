using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Luo_Painter.Brushes
{
    public static class InkExtensions
    {

        public static string GetResource(this InkType type) => $"ms-appx:///Luo Painter.Brushes/Icons/{type}Icon.xaml";
        public static ControlTemplate GetTemplate(this InkType type, ResourceDictionary resource) => resource[$"{type}Icon"] as ControlTemplate;

        public static string GetThumbnail(this string path) => $"ms-appx:///Luo Painter.Brushes/Thumbnails/{path}/Thumbnail.png";
        public static string GetThumbnailRender(this string path) => $"ms-appx:///Luo Painter.Brushes/Thumbnails/{path}/Render.png";

        public static string GetTexture(this string path) => $@"ms-appx:///Luo Painter.Brushes/Textures/{path}/Texture.png";
        public static string GetTextureSource(this string path) => $@"Luo Painter.Brushes/Textures/{path}/Source.png";

    }
}