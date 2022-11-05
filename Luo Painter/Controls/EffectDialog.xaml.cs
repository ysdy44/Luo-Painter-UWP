using Luo_Painter.Options;
using System;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class EffectDialog : ContentDialog
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick;
   
        //@Construct
        public EffectDialog()
        {
            this.InitializeComponent();
            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is OptionType item)
                {
                    this.ItemClick?.Invoke(this, item);//Delegate
                }
            };
        }

    }
}