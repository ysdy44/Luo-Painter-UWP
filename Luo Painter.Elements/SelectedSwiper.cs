using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Elements
{
    public sealed class SelectedSwiper : SwipeControl
    {
        ListViewItem Ancestor;
        readonly SwipeItem Item = new SwipeItem
        {
            BehaviorOnInvoked = SwipeBehaviorOnInvoked.Close,
        };

        //@Content
        public Visibility Visible { set => base.Opacity = value is Visibility.Visible ? 1 : 0.4; }

        public SelectedSwiper()
        {
            // base.Unloaded += (s, e) => this.Ancestor = null;
            base.Loaded += (s, e) => this.Ancestor = this.FindAncestor<ListViewItem>();
            base.RightItems = new SwipeItems { this.Item };
            base.RightItems.Mode = SwipeMode.Execute;
            this.Item.Invoked += (s, e) => this.Ancestor.Toggle();
        }
    }
}