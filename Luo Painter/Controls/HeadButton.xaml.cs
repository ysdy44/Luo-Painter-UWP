using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class HeadButton : StackPanel
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick { remove => this.Command.Click -= value; add => this.Command.Click += value; }

        //@Content
        public bool PasteIsEnabled { get; set; }

        //@Construct
        public HeadButton()
        {
            this.InitializeComponent();
            this.MenuFlyout.Opened += (s, e) =>
            {
                this.PasteItem.GoToState(this.PasteIsEnabled);
            };
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}