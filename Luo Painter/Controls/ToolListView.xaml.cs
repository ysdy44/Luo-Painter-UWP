﻿using Luo_Painter.Options;
using Windows.ApplicationModel.Resources;
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

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
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
                Content = item,
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
        }
    }
}