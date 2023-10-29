﻿using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    internal sealed class OptionIcon : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Resources.Source = new OptionResourceUri(value);
            base.Template = value.GetTemplate(base.Resources);
            base.Content = value;
            ToolTipService.SetToolTip(this, new ToolTip
            {
                Content = value,
            });
        }
    }

    internal sealed class BlendIcon : TIcon<BlendEffectMode>
    {
        protected override void OnTypeChanged(BlendEffectMode value)
        {
            base.Resources.Source = new Uri(value.GetResource());
            base.Content = new StackPanel
            {
                Spacing = 12,
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new ContentControl
                    {
                        Content = value,
                        Template = value.GetTemplate(base.Resources),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                    new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = value.ToString()
                    }
                }
            };
        }
    }


    internal sealed class OptionGroupingList : GroupingList<OptionGrouping, OptionType, OptionType> { }
    internal sealed class OptionGrouping : Grouping<OptionType, OptionType> { }

    internal sealed class BlendGroupingList : GroupingList<BlendGrouping, BlendEffectMode, BlendEffectMode> { }
    internal sealed class BlendGrouping : Grouping<BlendEffectMode, BlendEffectMode> { }


    public sealed partial class ListViewPage : Page
    {
        public ListViewPage()
        {
            this.InitializeComponent();
        }
    }
}