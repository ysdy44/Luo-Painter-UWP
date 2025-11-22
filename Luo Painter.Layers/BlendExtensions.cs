using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Layers
{
    public static class BlendExtensions
    {
        public static BlendEffectMode None = (BlendEffectMode)(-1); // None
        public static bool IsDefined(this BlendEffectMode type) => System.Enum.IsDefined(typeof(BlendEffectMode), type);

        public static string GetTitle(this BlendEffectMode type) => type.IsDefined() ? type.ToString() : "None";
        public static string GetIcon(this BlendEffectMode type) => type.IsDefined() ? type.ToString().First().ToString() : "N";
    }
}