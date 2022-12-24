using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Elements
{
    public enum PipItems
    {
        Others,
        Current,
        Last,
        Next,
    }

    [ContentProperty(Name = nameof(Children))]
    public sealed partial class PipsPivot2 : UserControl
    {

        public int Index { get; private set; }
        public UIElementCollection Children => this.RootCanvas.Children;

        public PipsPivot2()
        {
            this.InitializeComponent();
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                double width = e.NewSize.Width;
                double height = e.NewSize.Height;

                int count = this.RootCanvas.Children.Count;

                this.ChangeSize(count, width, height);
                if (base.IsLoaded)
                {
                    this.ChangeView(width);
                }
            };
            base.Loaded += (s, e) =>
            {
                int index = this.Index;
                int count = this.RootCanvas.Children.Count;

                double width = this.ScrollingHost.ActualWidth;

                if (this.ChangeView(width))
                {
                    this.ChangeTransform(index, count, width);
                }
            };
            this.ScrollingHost.DirectManipulationCompleted += (s, e) => this.Direct();
            this.ScrollingHost.ViewChanging += (s, e) => this.Direct();
        }

        private void Direct()
        {
            if (base.IsLoaded)
            {
                int index = this.Index;
                int count = this.RootCanvas.Children.Count;

                double width = this.ScrollingHost.ActualWidth;
                double offset = this.ScrollingHost.HorizontalOffset;

                if (this.ChangeIndex(index, count, width, offset))
                {
                    if (this.ChangeView(width))
                    {
                        this.ChangeTransform(index, count, width);
                    }
                }
            }
        }

        private void ChangeSize(int count, double width, double height)
        {
            this.RootStackPanel.Width = width * count;
            this.RootStackPanel.Height = height;
            foreach (FrameworkElement item in this.RootStackPanel.Children)
            {
                item.Width = width;
                item.Height = height;
            }

            this.RootCanvas.Width = width;
            this.RootCanvas.Height = height;
            foreach (FrameworkElement item in this.RootCanvas.Children)
            {
                item.Width = width;
                item.Height = height;
            }
        }

        private bool ChangeView(double horizontalOffset)
        {
            return this.ScrollingHost.ChangeView(horizontalOffset, null, null, true);
        }

        private bool ChangeIndex(int index, int count, double width, double offset, double tolerance = 15)
        {
            PipItems items;
            if (offset < tolerance)
                items = PipItems.Last;
            else if (offset > width + width - tolerance)
                items = PipItems.Next;
            else if (offset > width && offset < width + width)
                items = PipItems.Current;
            else
                items = PipItems.Others;

            switch (items)
            {
                case PipItems.Last:
                    this.Index = ((index % count) + count - 1) % count;
                    return true;
                case PipItems.Next:
                    this.Index = ((index % count) + count + 1) % count;
                    return true;
                default:
                    return false;
            }
        }

        private void ChangeTransform(int index, int count, double width)
        {
            for (int i = 0; i < count; i++)
            {
                PipItems items;
                if (i == index)
                    items = PipItems.Current;
                else if (i == index - 1 || (i == count - 1 && index == 0))
                    items = PipItems.Last;
                else if (i == index + 1 || (i == 0 && index == count - 1))
                    items = PipItems.Next;
                else
                    items = PipItems.Others;

                UIElement item = this.RootCanvas.Children[i];
                switch (items)
                {
                    case PipItems.Others:
                        item.Visibility = Visibility.Collapsed;
                        Canvas.SetLeft(item, 0);
                        break;
                    case PipItems.Current:
                        Canvas.SetLeft(item, 0);
                        item.Visibility = Visibility.Visible;
                        break;
                    case PipItems.Last:
                        Canvas.SetLeft(item, -width);
                        item.Visibility = Visibility.Visible;
                        break;
                    case PipItems.Next:
                        Canvas.SetLeft(item, width);
                        item.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

    }
}