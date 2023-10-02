using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Strings
{
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