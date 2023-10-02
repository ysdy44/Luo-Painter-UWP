using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
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
}