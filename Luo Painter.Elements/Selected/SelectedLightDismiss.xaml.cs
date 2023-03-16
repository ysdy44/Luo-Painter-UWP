using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Luo_Painter.Elements
{
    public sealed class SelectedLightDismissPresenter : Grid // ContentPresenter
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedLightDismiss.xaml.</summary>
        internal GridViewItem Ancestor;

        public SelectedLightDismissPresenter()
        {
            //base.Unloaded += (s, e) =>
            //{
            //    this.Remove();
            //};
            base.Loaded += (s, e) =>
            {
                if (this.FindAncestor<GridViewItem>() is GridViewItem item)
                {
                    this.Add(item);
                }
                else
                {
                    this.Remove();
                }
            };
        }
        private void Remove()
        {
            this.Ancestor = null;
            this.Children.Clear();
        }
        private void Add(GridViewItem item)
        {
            this.Ancestor = item;
            this.Children.Add(new SelectedLightDismiss(this));
        }
    }

    internal sealed partial class SelectedLightDismiss : UserControl
    {
        //@Converter
        private double BooleanToVisibilityConverter(bool value) => value ? 0.4 : 0;

        readonly SelectedLightDismissPresenter Presenter;

        internal SelectedLightDismiss(SelectedLightDismissPresenter presenter)
        {
            this.InitializeComponent();
            this.Presenter = presenter;
        }
    }
}