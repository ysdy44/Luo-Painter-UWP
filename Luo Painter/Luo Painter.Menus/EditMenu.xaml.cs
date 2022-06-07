using Luo_Painter.Options;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Menus
{
    internal class EditKeyboardAccelerator : KeyboardAccelerator
    {
        public OptionType CommandParameter { get; set; }
        public OptionTypeCommand Command { get; set; }
        public EditKeyboardAccelerator() => base.Invoked += (s, e) => this.Command.Execute(this.CommandParameter);
    }

    internal sealed class EditIcon : TButton<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            Edits.EditType editType = default;
            switch (value)
            {
                case OptionType.MarqueeInvert: editType = Edits.EditType.Invert; break;
                case OptionType.MarqueeTransform: editType = Edits.EditType.Transform; break;
                case OptionType.CropCanvas: editType = Edits.EditType.Crop; break;
                default: if (System.Enum.TryParse(typeof(Edits.EditType), value.ToString(), out object obj)) editType = (Edits.EditType)obj; break;
            }

            base.Content = editType.ToString();
            base.Template = Edits.EditExtensions.GetTemplate(editType, out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

    internal sealed class EditItem : TButton<OptionType>
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

        protected override void OnTypeChanged(OptionType value)
        {
            Edits.EditType editType = default;
            switch (value)
            {
                case OptionType.MarqueeInvert: editType = Edits.EditType.Invert; break;
                case OptionType.MarqueeTransform: editType = Edits.EditType.Transform; break;
                case OptionType.CropCanvas: editType = Edits.EditType.Crop; break;
                default: if (System.Enum.TryParse(typeof(Edits.EditType), value.ToString(), out object obj)) editType = (Edits.EditType)obj; break;
            }

            base.CommandParameter = editType;
            this.Icon = new ContentControl
            {
                Content = editType,
                Template = Edits.EditExtensions.GetTemplate(editType, out ResourceDictionary resource),
                Resources = resource,
            };
            this.Icon.GoToState(base.IsEnabled);
            base.Content = TIconExtensions.GetGrid(this.Icon, editType.ToString());
        }
    }

    public sealed partial class EditMenu : Expander
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.Command.Click -= value;
            add => this.Command.Click += value;
        }
        public void Execute(OptionType item) => this.Command.Execute(item);

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