using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Provides a <see cref="ListView"/> with a reorder animation.
    /// </summary>
    public sealed class AnimationListView : ListView
    {
        ScrollViewer ScrollViewer;
        ItemsPresenter ItemsPresenter;

        readonly Visual ElementVisual;
        readonly Compositor Compositor;
        readonly Vector3KeyFrameAnimation OffsetAnimation;
        readonly ImplicitAnimationCollection Animation;

        ~AnimationListView() => this.Animation.Remove(nameof(Visual.Offset));
        //@Construct
        /// <summary>
        /// Constructs a <see cref="ListView"/> instance with reorder animation.
        /// </summary>
        public AnimationListView()
        {
            this.ElementVisual = ElementCompositionPreview.GetElementVisual(this);
            this.Compositor = this.ElementVisual.Compositor;

            this.OffsetAnimation = this.Compositor.CreateVector3KeyFrameAnimation();
            this.OffsetAnimation.Target = nameof(Visual.Offset);
            this.OffsetAnimation.Duration = System.TimeSpan.FromMilliseconds(300);
            this.OffsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");

            this.Animation = this.Compositor.CreateImplicitAnimationCollection();
            this.Animation[nameof(Visual.Offset)] = this.OffsetAnimation;

            base.ChoosingItemContainer += (s, args) =>
            {
                if (args.ItemContainer is null) return;

                // Pokes the Z index of each container when one is chosen, to ensure animations are displayed correctly.
                args.ItemContainer.PokeZIndex();
            };
            base.ContainerContentChanging += (s, args) =>
            {
                if (args.InRecycleQueue)
                {
                    // Pokes the Z index of each container when one is chosen, to ensure animations are displayed correctly.
                    args.ItemContainer.PokeZIndex();
                    return;
                }

                // Updates the reorder animation to each container, whenever one changes.
                Visual visual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
                visual.ImplicitAnimations = this.Animation;
            };

            base.Loaded += async (s, e) =>
            {
                if (this.ScrollViewer is null) return;
                await Task.Delay(200);

                base.Margin = new Thickness(0, 50, 0, 0);
                this.ScrollViewer.ChangeView(null, 50, null);
                //this.ScrollViewer.VerticalSnapPointsType = SnapPointsType.Mandatory;
                //this.ScrollViewer.VerticalSnapPointsAlignment = SnapPointsAlignment.Near;
            };
        }

        public void Resizing(Size size)
        {
            // Align Center
            int p = (int)size.Width % 210;
            base.Padding = new Thickness(p / 2, 0, p / 2, 0);

            if (this.ItemsPresenter is null) return;
            this.ItemsPresenter.MinHeight = size.Height + 50;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.ScrollViewer is null is false) this.ScrollViewer.DirectManipulationCompleted -= this.ScrollViewer_DirectManipulationCompleted;
            this.ScrollViewer = base.GetTemplateChild(nameof(ScrollViewer)) as ScrollViewer;
            if (this.ScrollViewer is null) return;
            this.ScrollViewer.DirectManipulationCompleted += this.ScrollViewer_DirectManipulationCompleted;

            this.ItemsPresenter = this.ScrollViewer.Content as ItemsPresenter;
        }

        private void ScrollViewer_DirectManipulationCompleted(object sender, object e)
        {
            double y = this.ScrollViewer.VerticalOffset;
            if (y < 50)
            {
                this.ScrollViewer.ChangeView(null, 0, null);
            }
            else if (y < 50 + 50 + 50)
            {
                this.ScrollViewer.ChangeView(null, 50, null);
            }
        }
    }
}