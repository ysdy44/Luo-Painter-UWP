using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ToolListView : XamlListView
    {

        public OptionType SelectedItem
        {
            get => (base.SelectedItem is OptionType item) ? item : default;
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