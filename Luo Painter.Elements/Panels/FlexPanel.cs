using Windows.Foundation;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed class FlexPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0;
            double height = 0;
            foreach (UIElement child in base.Children)
            {
                child.Measure(availableSize);
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }
            return new Size(width, height);
        }
        protected override Size ArrangeOverride(Size availableSize)
        {
            int index = 0;
            double itemWidth = availableSize.Width / base.Children.Count;
            foreach (UIElement child in base.Children)
            {
                child.Arrange(new Rect(index * itemWidth, 0, itemWidth, availableSize.Height));
                index++;
            }
            return availableSize;
        }
    }
}