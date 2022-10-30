using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;

namespace Luo_Painter.Controls
{
    public sealed partial class LayerMenu : Expander
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }

        public bool PasteIsEnabled
        {
            get => this.PasteItem.IsEnabled;
            set => this.PasteItem.IsEnabled = value;
        }

        public object SelectedItem
        {
            set
            {
                if (value is null)
                {
                    this.CutItem.IsEnabled = false;
                    this.CopyItem.IsEnabled = false;
                    this.RemoveItem.IsEnabled = false;
                }
                else
                {
                    this.CutItem.IsEnabled = true;
                    this.CopyItem.IsEnabled = true;
                    this.RemoveItem.IsEnabled = true;
                }
            }
        }

        //@Construct
        public LayerMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}