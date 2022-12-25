using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.HSVColorPickers
{
    public abstract class ButtonSelector<TKey> : TSelector<TKey, Button>
    {
        
        //@Delegate
        public event RoutedEventHandler ItemClick;
        public event ManipulationStartedEventHandler ItemManipulationStarted;
        public event ManipulationDeltaEventHandler ItemManipulationDelta;
        public event ManipulationCompletedEventHandler ItemManipulationCompleted;
        public event KeyEventHandler ItemPreviewKeyDown;

        //@Override
        protected override Button CreateItem(TKey key) => new Button
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