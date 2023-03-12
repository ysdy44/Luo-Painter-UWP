using Luo_Painter.Layers;
using Luo_Painter.Models;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class UIExtension : MarkupExtension
    {
        public UIType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }


    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class OptionExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }

    [MarkupExtensionReturnType(ReturnType = typeof(OptionType))]
    public class OptionTypeExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue() => this.Type;
    }

    [MarkupExtensionReturnType(ReturnType = typeof(ContentControl))]
    public class OptionIconExtension : MarkupExtension
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
    public class OptionItemExtension : MarkupExtension
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
    public class ElementExtension : MarkupExtension
    {
        public ElementType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
    }

    [MarkupExtensionReturnType(ReturnType = typeof(ContentControl))]
    public class ElementIconExtension : MarkupExtension
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
    public class ElementItemExtension : MarkupExtension
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
    public class BlendItemExtension : MarkupExtension
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
    public class InterpolationItemExtension : MarkupExtension
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
                        Text = App.Resource.GetString($"Interpolations_{this.Type}"),
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