using Luo_Painter.Edits;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Menus
{
    internal class EditTypeCommand : RelayCommand<EditType> { }

    internal class EditKeyboardAccelerator : KeyboardAccelerator
    {
        public EditType CommandParameter { get; set; }
        public EditTypeCommand Command { get; set; }
        public EditKeyboardAccelerator() => base.Invoked += (s, e) => this.Command.Execute(this.CommandParameter);
    }

    internal sealed class EditIcon : TButton<EditType>
    {
        protected override void OnTypeChanged(EditType value)
        {
            base.Content = value.ToString();
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

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

    public sealed partial class EditMenu : Expander
    {
        //@Delegate
        public event EventHandler<EditType> ItemClick
        {
            remove => this.EditTypeCommand.Click -= value;
            add => this.EditTypeCommand.Click += value;
        }
        public void Execute(EditType item) => this.EditTypeCommand.Execute(item);

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