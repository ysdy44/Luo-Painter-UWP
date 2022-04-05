using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.TestApp
{
    public sealed class SelectedButtonPresenter : ContentPresenter
    {
        public SelectedButtonPresenter()
        {
            base.Unloaded += (s, e) => this.Content = null;
            base.Loaded += (s, e) => this.Content = new SelectedButton(this);
        }
    }

    internal sealed partial class SelectedButton : Button
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedButton.xaml.</summary>
        readonly ListViewItem Ancestor; 
     
        internal SelectedButton(SelectedButtonPresenter presenter)
        {
            this.InitializeComponent();
            this.Ancestor = SelectedButton.FindAncestor(presenter);
            base.Click += (s, e) => this.Ancestor.IsSelected = !this.Ancestor.IsSelected;
        }

        //@Static
        private static ListViewItem FindAncestor(DependencyObject reference)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            if (parent == null) return null;
            else if (parent is ListViewItem result) return result;
            else return SelectedButton.FindAncestor(parent);
        }
    }
}