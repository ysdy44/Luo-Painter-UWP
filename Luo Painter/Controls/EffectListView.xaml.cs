using Luo_Painter.Options;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class EffectListView : UserControl
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick;

        //@Converter
        private object ItemsSourceConverter(int value) => this.Collection[value];
        OptionType SelectedType;

        //@Construct
        public EffectListView()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is OptionType item)
                {
                    this.SelectedType = item;
                    this.ItemClick?.Invoke(this, item);
                }
            };
            this.ApplyButton.Click += (s, e) =>
            {
                if (this.SelectedType.IsEffect())
                {
                    this.ItemClick?.Invoke(this, this.SelectedType);
                }
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}