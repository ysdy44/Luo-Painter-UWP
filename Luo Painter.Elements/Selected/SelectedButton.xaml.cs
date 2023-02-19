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
            // base.Unloaded += (s, e) => this.Children.Clear();
            base.Loaded += (s, e) => this.Children.Add(new SelectedButton(this.FindAncestor<ListViewItem>()));
        }
    }

    internal sealed partial class SelectedButton : Button
    {

        /// <summary> Please binding the <see cref="SelectorItem.IsSelected" /> of Ancestor in SelectedButton.xaml.</summary>
        readonly ListViewItem Ancestor;

        internal SelectedButton(ListViewItem ancestor)
        {
            this.InitializeComponent();
            this.Ancestor = ancestor;
            base.Click += (s, e) => this.Ancestor.Toggle();
        }
    }
}