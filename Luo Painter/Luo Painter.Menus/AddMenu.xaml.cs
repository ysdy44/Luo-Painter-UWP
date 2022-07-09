using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Menus
{
    public sealed partial class AddMenu : Expander
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }

        //@Construct
        public AddMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}