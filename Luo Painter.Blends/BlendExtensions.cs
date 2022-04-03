using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Blends
{
    public static class BlendExtensions
    {

        public static BlendEffectMode None = (BlendEffectMode)(-1); // None
        public static bool IsNone(this BlendEffectMode type) => (int)type == -1;

        public static ControlTemplate GetTemplate(this BlendEffectMode type, out ResourceDictionary resource, out string title)
        {
            if (type.IsNone())
            {
                title = "None";
                resource = new ResourceDictionary
                {
                    Source = new Uri($"ms-appx:///Luo Painter.Blends/Icons/NoneIcon.xaml")
                };
                return resource[$"NoneIcon"] as ControlTemplate;
            }
            else
            {
                title = $"{type}";
                resource = new ResourceDictionary
                {
                    Source = new Uri($"ms-appx:///Luo Painter.Blends/Icons/{title}Icon.xaml")
                };
                return resource[$"{title}Icon"] as ControlTemplate;
            }
        }

    }
}