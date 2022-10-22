using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Controls
{
    public sealed partial class AdjustmentMenu : Expander
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }

        //@Construct
        public AdjustmentMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}