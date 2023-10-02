using Luo_Painter.Controls;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
    public static class StringExtensions
    {
        // 0
        public static string GetString(this OptionType type)
        {
            return App.Resource.GetString(type.ToString());
        }
        public static string GetString(this ElementType type)
        {
            return App.Resource.GetString(type.ToString());
        }
        public static string GetString(this UIType type)
        {
            return App.Resource.GetString(type.ToString());
        }

        // 1
        public static string GetString(this BlendEffectMode type)
        {
            return App.Resource.GetString($"Blends_{type.GetTitle()}");
        }
        public static string GetString(this LayerType type)
        {
            return App.Resource.GetString($"Layer_{type}");
        }

        // 2
        public static string GetString(this InkGroupingType type)
        {
            if (type.HasFlag(InkGroupingType.Others))
            {
                return App.Resource.GetString($"Brush_{InkGroupingType.Others}");
            }
            else
            {
                return App.Resource.GetString($"Brush_{type}");
            }
        }

        // 3
        public static string GetString(this CanvasImageInterpolation type)
        {
            return App.Resource.GetString($"Interpolation_{type}");
        }
        public static string GetString(this TipType type, bool isSub = false)
        {
            if (isSub)
            {
                return App.Resource.GetString($"SubTip_{type}");
            }
            else
            {
                return App.Resource.GetString($"Tip_{type}");
            }
        }


        public static MessageDialog ToDialog(this TipType type)
        {
            return new MessageDialog
            (
                App.Resource.GetString($"SubTip_{type}"),
                App.Resource.GetString($"Tip_{type}")
            );
        }
        public static MessageDialog ToDialog(this TipType type, string subtip)
        {
            return new MessageDialog
            (
                subtip,
                App.Resource.GetString($"Tip_{type}")
            );
        }
    }
}