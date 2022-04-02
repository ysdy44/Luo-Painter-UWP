using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Tools
{
    public static class ToolExtensions
    {

        public static ControlTemplate GetTemplate(this ToolType type, out ResourceDictionary resource)
        {
            resource = new ResourceDictionary
            {
                Source = new Uri($"ms-appx:///Luo Painter.Tools/Icons/{type}Icon.xaml")
            };
            return resource[$"{type}Icon"] as ControlTemplate;
        }

        public static Color GetColor(this ToolGroupType groupType)
        {
            switch (groupType)
            {
                case ToolGroupType.Pixel: return Color.FromArgb(255, 153, 50, 204);
                case ToolGroupType.Paint: return Color.FromArgb(255, 255, 0, 0);

                case ToolGroupType.Liquefy: return Color.FromArgb(255, 255, 165, 0);
                case ToolGroupType.Adjust: return Color.FromArgb(255, 255, 215, 0);

                case ToolGroupType.Vector: return Color.FromArgb(255, 173, 255, 47);
                case ToolGroupType.Geometry: return Color.FromArgb(255, 67, 158, 251);
                case ToolGroupType.Curve: return Color.FromArgb(255, 0, 191, 255);
                case ToolGroupType.Text: return Colors.White;
                case ToolGroupType.ThreeDimensions: return Color.FromArgb(255, 255, 0, 255);

                case ToolGroupType.Crop: return Color.FromArgb(255, 100, 100, 237);

                default: return Colors.Transparent;
            }
        }

    }
}