using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter
{
    internal sealed class BlendList : List<BlendEffectMode> { }
    internal sealed class BlendGroupingList : List<BlendEffectMode> { }
    internal class BlendItem : TItem<BlendEffectMode>
    {
        protected override void OnTypeChanged(BlendEffectMode value)
        {
            base.TextBlock.Text = App.Resource.GetString($"Blends_{value.GetTitle()}");
            base.Icon.Content = value.GetIcon();
        }
    }

    internal sealed class HardnessList : List<BrushEdgeHardness> { }
    internal sealed class HardnessIcon : TIcon<BrushEdgeHardness>
    {
        protected override void OnTypeChanged(BrushEdgeHardness value)
        {
            base.Content = App.Resource.GetString($"Hardness_{value}");
        }
    }

    internal sealed class InterpolationItem : TItem<CanvasImageInterpolation>
    {
        protected override void OnTypeChanged(CanvasImageInterpolation value)
        {
            base.TextBlock.Text = App.Resource.GetString($"Interpolation_{value}");
            base.Icon.Content = value.ToString().First().ToString();
        }
    }

    internal sealed class OptionGroupingList : GroupingList<OptionGrouping, OptionType, OptionType> { }
    internal class OptionGrouping : Grouping<OptionType, OptionType> { }

    internal class OptionIcon : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = value;
            base.Resources.Source = new Uri(value.GetResource());
            base.Template = value.GetTemplate(base.Resources);
        }
    }

    internal sealed class OptionThumbnail : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = App.Resource.GetString(value.ToString());

            // https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media
            if (value.ExistThumbnail())
            {
                BitmapImage bitmap = new BitmapImage();
                base.Background = new ImageBrush
                {
                    ImageSource = bitmap
                };
                bitmap.UriSource = new Uri(value.GetThumbnail());
            }
        }
    }
}