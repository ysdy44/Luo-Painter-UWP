using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Luo_Painter.Elements
{
    public class GradientStopSelector : TSelector<GradientStop, Button>
    {

        //@Delegate
        public event RoutedEventHandler ItemClick;
        public event ManipulationStartedEventHandler ItemManipulationStarted;
        public event ManipulationDeltaEventHandler ItemManipulationDelta;
        public event ManipulationCompletedEventHandler ItemManipulationCompleted;
        public event KeyEventHandler ItemPreviewKeyDown;

        #region DependencyProperty


        /// <summary> Gets or sets style of <see cref = "GradientStopSelector" />'s item. </summary>
        public Style ItemStyle
        {
            get => (Style)base.GetValue(ItemStyleProperty);
            set => base.SetValue(ItemStyleProperty, value);
        }
        /// <summary> Identifies the <see cref = "GradientStopSelector.ItemStyle" /> dependency property. </summary>
        public static readonly DependencyProperty ItemStyleProperty = DependencyProperty.Register(nameof(ItemStyle), typeof(Style), typeof(GradientStopSelector), new PropertyMetadata(null));


        /// <summary> Gets or sets template of <see cref = "GradientStopSelector" />'s item. </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)base.GetValue(ItemTemplateProperty);
            set => base.SetValue(ItemTemplateProperty, value);
        }
        /// <summary> Identifies the <see cref = "GradientStopSelector.ItemTemplate" /> dependency property. </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(GradientStopSelector), new PropertyMetadata(null));


        #endregion

        //@Override
        public override double GetItemLeft(GradientStop key) => key.Offset * base.ActualWidth - 25;
        public override double GetItemTop(GradientStop key) => 0;

        protected override Button CreateItem(GradientStop key) => new Button
        {
            Content = key
        };
        protected override void RemoveItemHandle(Button element)
        {
            element.Style = null;
            element.ContentTemplate = null;
            element.Click -= this.ItemClick;
            element.ManipulationMode = ManipulationModes.None;
            element.ManipulationStarted -= this.ItemManipulationStarted;
            element.ManipulationDelta -= this.ItemManipulationDelta;
            element.ManipulationCompleted -= this.ItemManipulationCompleted;
            element.PreviewKeyDown -= this.ItemPreviewKeyDown;
        }
        protected override void AddItemHandle(Button element)
        {
            element.Style = this.ItemStyle;
            element.ContentTemplate = this.ItemTemplate;
            element.Click += this.ItemClick;
            element.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            element.ManipulationStarted += this.ItemManipulationStarted;
            element.ManipulationDelta += this.ItemManipulationDelta;
            element.ManipulationCompleted += this.ItemManipulationCompleted;
            element.PreviewKeyDown += this.ItemPreviewKeyDown;
        }
    }
}