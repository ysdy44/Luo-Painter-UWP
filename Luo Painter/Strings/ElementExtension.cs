using Luo_Painter.Models;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ElementExtension : MarkupExtension
    {
        public ElementType Type { get; set; }
        protected override object ProvideValue() => this.Type.GetString();
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
                        Text =this.Type.GetString(),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis,
                    },
                    icon
                }
            };
        }
    }
}