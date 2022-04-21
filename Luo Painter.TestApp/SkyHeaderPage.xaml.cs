using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Luo_Painter.TestApp
{
    public sealed partial class SkyHeaderPage : Page
    {
        double StartingVerticalOffset;
        public SkyHeaderPage()
        {
            this.InitializeComponent();
            base.Loaded += (s, e) =>
            {
                CompositionPropertySet scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(this.ScrollViewer);
                Compositor compositor = scrollerPropertySet.Compositor;

                // double.MinValue < scroller.Translation.Y <= 0
                // string progress = "- scroller.Translation.Y"; // No Move
                // string progress = "- scroller.Translation.Y + 100"; // Offset.Y 100
                string progress = "Max( 150, - scroller.Translation.Y ) - 150"; // 50 = 200 - 150
                ExpressionAnimation offsetExpression = compositor.CreateExpressionAnimation(progress);
                offsetExpression.SetReferenceParameter("scroller", scrollerPropertySet);

                Visual headerVisual = ElementCompositionPreview.GetElementVisual(this.ScrollHeader);
                headerVisual.StartAnimation("Offset.Y", offsetExpression);
            };

            this.CloseButton.Click += (s, e) =>
            {
                int y = (this.ScrollViewer.VerticalOffset == 0) ? 150 : 0;
                this.ScrollViewer.ChangeView(null, y, null, disableAnimation: false);
            };

            this.TitleGrid.ManipulationStarted += (s, e) =>
            {
                this.StartingVerticalOffset = this.ScrollViewer.VerticalOffset;
            };
            this.TitleGrid.ManipulationDelta += (s, e) =>
            {
                double y = this.StartingVerticalOffset - e.Cumulative.Translation.Y;
                this.ScrollViewer.ChangeView(null, y, null, disableAnimation: true);
            };
            this.TitleGrid.ManipulationCompleted += (s, e) =>
            {
                double y = this.StartingVerticalOffset - e.Cumulative.Translation.Y;
                this.ScrollViewer.ChangeView(null, y, null, disableAnimation: true);
            };
        }
    }
}