using Luo_Painter.Options;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class HeadButton : StackPanel
    {
        //@Command
        public ICommand Command { get; set; }

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