using Luo_Painter.Options;
using Luo_Painter.Elements;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Luo_Painter.Menus
{
    internal sealed class EditButton : TButton<OptionType>
    {

        Control Icon;
        public EditButton()
        {
            base.Loaded += (s, e) =>
            {
                if (this.Icon is null) return;
                this.Icon.GoToState(base.IsEnabled);
            };
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
            this.Icon = new ContentControl
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            };
            this.Icon.GoToState(base.IsEnabled);
            base.Content = this.Icon;
            base.CommandParameter = value;
            base.HorizontalContentAlignment = HorizontalAlignment.Center;
            base.VerticalContentAlignment = VerticalAlignment.Center;
            ToolTipService.SetToolTip(this, new ToolTip
            {
                Content = value.ToString(),
                Style = App.Current.Resources["AppToolTipStyle"] as Style
            });
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
            base.CommandParameter = value;
            this.Icon = new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
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