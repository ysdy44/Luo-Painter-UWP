using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class MoreOptionButton : StackPanel
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
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}