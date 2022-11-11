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
        public bool IsExist
        {
            set
            {
                base.IsHitTestVisible = value;
                if (this.Ancestor is null) return;
                this.Ancestor.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public SelectedSwiper()
        {
            // base.Unloaded += (s, e) => this.Ancestor = null;
            base.Loaded += (s, e) =>
            {
                this.Ancestor = this.FindAncestor<ListViewItem>();
                if (this.Ancestor is null) return;
                this.Ancestor.Visibility = base.IsHitTestVisible ? Visibility.Visible : Visibility.Collapsed;
            };
            //@Debug
            // Left is better
            //base.RightItems = new SwipeItems { this.Item };
            //base.RightItems.Mode = SwipeMode.Execute;
            base.LeftItems = new SwipeItems { this.Item };
            base.LeftItems.Mode = SwipeMode.Execute;
            this.Item.Invoked += (s, e) => this.Ancestor.Toggle();
        }
    }
}