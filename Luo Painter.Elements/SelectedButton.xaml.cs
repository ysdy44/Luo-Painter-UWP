using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    // ContentPresenter : 
    // In a ListViewItem,
    // if it's based on a ContentPresenter or ContentControl,
    // it crashes on dragging items.
    //
    // Grid : 
    // OK, but why?
    public sealed class SelectedButtonPresenter : Grid // ContentPresenter
    {
        public SelectedButtonPresenter()
        {
            // base.Unloaded += (s, e) => this.Content = null;
            base.Loaded += (s, e) => this.Children.Add(new SelectedButton(this));
        }

        //@Static
        public static ListViewItem FindAncestor(DependencyObject reference)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            if (parent == null) return null;
            else if (parent is ListViewItem result) return result;
            else return SelectedButtonPresenter.FindAncestor(parent);
        }
    }

    internal sealed partial class SelectedButton : Button
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedButton.xaml.</summary>
        readonly ListViewItem Ancestor;

        internal SelectedButton(SelectedButtonPresenter presenter)
        {
            this.InitializeComponent();
            this.Ancestor = SelectedButtonPresenter.FindAncestor(presenter);
            base.Click += (s, e) => this.Ancestor.IsSelected = !this.Ancestor.IsSelected;
        }
    }
}