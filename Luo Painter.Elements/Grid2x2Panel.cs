using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed class Grid2x2Panel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double w = availableSize.Width / 2;
            double h = availableSize.Height / 2;
            foreach (UIElement child in base.Children)
            {
                child.Measure(new Size(w, h));
            }
            return availableSize;
        }
        protected override Size ArrangeOverride(Size availableSize)
        {
            double w = availableSize.Width / 2;
            double h = availableSize.Height / 2;
            for (int i = 0; i < base.Children.Count; i++)
            {
                switch (i)
                {
                    case 1: base.Children[i].Arrange(new Rect(w, 0, w, h)); break;
                    case 2: base.Children[i].Arrange(new Rect(0, h, w, h)); break;
                    case 3: base.Children[i].Arrange(new Rect(w, h, w, h)); break;
                    default: base.Children[i].Arrange(new Rect(0, 0, w, h)); break;
                }
            }
            return availableSize;
        }
    }
}