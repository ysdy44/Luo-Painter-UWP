using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class MoreOptionButton : Button
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.OptionTypeCommand.Click -= value;
            add => this.OptionTypeCommand.Click += value;
        }

        //@Construct
        public MoreOptionButton()
        {
            this.InitializeComponent();
            this.ItemClick += (s, e) => this.MoreOptionFlyout.Hide();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}