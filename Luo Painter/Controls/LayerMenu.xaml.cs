using Luo_Painter.Blends;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas.Effects;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using System;
using System.Collections.Generic;

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

        //@Construct
        public LayerMenu()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void SetSelectedItem(ILayer layer)
        {
            if (layer is null)
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
}