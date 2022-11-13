using Luo_Painter.Options;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Controls
{
    public sealed partial class ToolListView : XamlListView
    {

        //@Debug
        // SelectedIndex is always -1,
        // If the ToolListView is not Visible.
        /*
        public OptionType SelectedItem
        {
            get => (base.SelectedItem is OptionType item) ? item : default;
            set => base.SelectedIndex = this.Collection.IndexOf(value);
        }
        */

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

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}