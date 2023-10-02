using Luo_Painter.Models;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
    [MarkupExtensionReturnType(ReturnType = typeof(OptionType))]
    public sealed class OptionTypeExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue() => this.Type;
    }

    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class OptionExtension : MarkupExtension
    {
        public OptionType Type { get; set; }
        protected override object ProvideValue() => App.Resource.GetString(this.Type.ToString());
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
}