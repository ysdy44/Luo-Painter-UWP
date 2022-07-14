using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Luo_Painter.Elements
{
    /// <summary>
    /// Provides a <see cref="GridView"/> with a reorder animation.
    /// </summary>
    public sealed class AnimationListView : ListView
    {
        readonly Visual ElementVisual;
        readonly Compositor Compositor;
        readonly Vector3KeyFrameAnimation OffsetAnimation;
        readonly ImplicitAnimationCollection Animation;
        ~AnimationListView() => this.Animation.Remove(nameof(Visual.Offset));
        //@Construct
        /// <summary>
        /// Constructs a <see cref="GridView"/> instance with reorder animation.
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
        }
    }
}