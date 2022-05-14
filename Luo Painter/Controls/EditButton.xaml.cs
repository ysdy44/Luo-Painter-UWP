using Luo_Painter.Edits;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal class EditTypeCommand : RelayCommand<EditType> { }

    internal sealed class EditItem : TButton<EditType>
    {

        Control Icon;
        public EditItem()
        {
            base.IsEnabledChanged += (s, e) =>
            {
                if (this.Icon is null) return;
                if (e.NewValue is bool value)
                {
                    this.Icon.GoToState(value);
                }
            };
        }

        protected override void OnTypeChanged(EditType value)
        {
            base.CommandParameter = value;
            this.Icon = new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            };
            this.Icon.GoToState(base.IsEnabled);
            base.Content = TIconExtensions.GetGrid(this.Icon, value.ToString());
        }
    }

    public sealed partial class EditButton : Grid
    {
        //@Delegate
        public event EventHandler<EditType> ItemClick
        {
            remove => this.EditTypeCommand.Click -= value;
            add => this.EditTypeCommand.Click += value;
        }

        //@Construct
        public EditButton()
        {
            this.InitializeComponent();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}