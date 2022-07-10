using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Blends
{
    public static class BlendExtensions
    {

        public static BlendEffectMode None = (BlendEffectMode)(-1); // None
        public static bool IsDefined(this BlendEffectMode type) => System.Enum.IsDefined(typeof(BlendEffectMode), type);

        public static string GetTitle(this BlendEffectMode type) => type.IsDefined() ? type.ToString() : "None";
        public static string GetResource(this BlendEffectMode type) => $"ms-appx:///Luo Painter.Blends/Icons/{type.GetTitle()}Icon.xaml";
        public static ControlTemplate GetTemplate(this BlendEffectMode type, ResourceDictionary resource) => resource[$"{type.GetTitle()}Icon"] as ControlTemplate;
        public static ControlTemplate GetTemplate(this BlendEffectMode type, out ResourceDictionary resource, out string title)
        {
            title = type.GetTitle();
            resource = new ResourceDictionary
            {
                Source = new Uri($"ms-appx:///Luo Painter.Blends/Icons/{title}Icon.xaml")
            };
            return resource[$"{title}Icon"] as ControlTemplate;
        }

        public static Color ToColor(this TagType tagType)
        {
            switch (tagType)
            {
                case TagType.None: return Colors.Transparent;
                case TagType.Red: return Colors.LightCoral;
                case TagType.Orange: return Colors.Orange;
                case TagType.Yellow: return Colors.Yellow;
                case TagType.Green: return Colors.YellowGreen;
                case TagType.Blue: return Colors.SkyBlue;
                case TagType.Purple: return Colors.Plum;

                default: return Colors.LightGray;
            }
        }

    }
}