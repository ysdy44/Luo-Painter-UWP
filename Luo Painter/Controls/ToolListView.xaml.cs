using Luo_Painter.Models;
using Luo_Painter.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ToolListView : XamlGridView
    {

        //@Content
        public OptionType SelectedType
        {
            get => (base.SelectedItem is OptionType item) ? item : OptionType.PaintBrush;
            set => base.SelectedIndex = this.Collection.IndexOf(value);
        }

        //@Construct
        public ToolListView()
        {
            this.InitializeComponent();
        }

        //@Override
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            ToolTipService.SetToolTip(element, null);
        }
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ToolTipService.SetToolTip(element, new ToolTip
            {
                Content = App.Resource.GetString(item.ToString()),
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
        }
    }
}