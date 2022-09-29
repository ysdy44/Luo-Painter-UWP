using Luo_Painter.Options;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Menus
{
    public sealed partial class EditMenu : Expander
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }
        public void Execute(OptionType item) => this.Command.Execute(item);
     
        public bool PasteIsEnabled
        {
            get => this.PasteItem.IsEnabled;
            set => this.PasteItem.IsEnabled = value;
        }

        //@Construct
        public EditMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}