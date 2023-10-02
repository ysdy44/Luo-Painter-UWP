using Luo_Painter.Controls;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
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
    }

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class UIExtension : MarkupExtension
    {
        public UIType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class OptionExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }
    [MarkupExtensionReturnType(ReturnType = typeof(OptionType))]
    public sealed class OptionTypeExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue() => this.Type;
    }
    [MarkupExtensionReturnType(ReturnType = typeof(ContentControl))]
    public sealed class OptionIconExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue()
        {
            ContentControl icon = new ContentControl
            {
                Content = this.Type
            };
            icon.Resources.Source = new Uri(this.Type.GetResource());
            icon.Template = this.Type.GetTemplate(icon.Resources);
            return icon;
        }
    }
    [MarkupExtensionReturnType(ReturnType = typeof(Grid))]
    public sealed class OptionItemExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue()
        {
            ContentControl icon = new ContentControl
            {
                Content = this.Type,
                Width = 32,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            icon.Resources.Source = new Uri(this.Type.GetResource());
            icon.Template = this.Type.GetTemplate(icon.Resources);

            return new Grid
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = App.Resource.GetString(this.Type.ToString()),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                    },
                    icon
                }
            };
        }
    }

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ElementExtension : MarkupExtension
    {
        public ElementType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }
    [MarkupExtensionReturnType(ReturnType = typeof(ContentControl))]
    public sealed class ElementIconExtension : MarkupExtension
    {
        public ElementType Type { get; set; }
        protected override object ProvideValue()
        {
            ContentControl icon = new ContentControl
            {
                Content = this.Type
            };
            icon.Resources.Source = new Uri(this.Type.GetResource());
            icon.Template = this.Type.GetTemplate(icon.Resources);
            return icon;
        }
    }
    [MarkupExtensionReturnType(ReturnType = typeof(Grid))]
    public sealed class ElementItemExtension : MarkupExtension
    {
        public ElementType Type { get; set; }
        protected override object ProvideValue()
        {
            ContentControl icon = new ContentControl
            {
                Content = this.Type,
                Width = 32,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            icon.Resources.Source = new Uri(this.Type.GetResource());
            icon.Template = this.Type.GetTemplate(icon.Resources);

            return new Grid
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = App.Resource.GetString(this.Type.ToString()),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                    },
                    icon
                }
            };
        }
    }

    [MarkupExtensionReturnType(ReturnType = typeof(Grid))]
    public sealed class BlendItemExtension : MarkupExtension
    {
        public BlendEffectMode Type { get; set; }
        protected override object ProvideValue()
        {
            return new Grid
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = App.Resource.GetString($"Blends_{this.Type.GetTitle()}"),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                    },
                    new ContentControl
                    {
                        Content = this.Type.GetIcon(),
                        Width = 32,
                        FontWeight = Windows.UI.Text.FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                    }
                }
            };
        }
    }

    [MarkupExtensionReturnType(ReturnType = typeof(Grid))]
    public sealed class InterpolationItemExtension : MarkupExtension
    {
        public CanvasImageInterpolation Type { get; set; }
        protected override object ProvideValue()
        {
            return new Grid
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = App.Resource.GetString($"Interpolation_{this.Type}"),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                    },
                    new ContentControl
                    {
                        Content = this.Type.ToString().First().ToString(),
                        Width = 32,
                        FontWeight = Windows.UI.Text.FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                    }
                }
            };
        }
    }
}