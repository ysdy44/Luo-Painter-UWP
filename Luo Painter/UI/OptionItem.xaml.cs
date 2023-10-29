using Luo_Painter.Models;
using Luo_Painter.Strings;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.UI
{
    public sealed partial class OptionItem : MenuFlyoutItem
    {

        #region DependencyProperty

        /// <summary> Gets or set the type for <see cref="OptionItem"/>. </summary>
        public OptionType Type
        {
            get => (OptionType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "OptionItem.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(OptionType), typeof(OptionItem), new PropertyMetadata(OptionType.Node, (sender, e) =>
        {
            OptionItem control = (OptionItem)sender;

            if (e.NewValue is OptionType value)
            {
                control.Update(value);
            }
        }));

        #endregion

        public OptionItem()
        {
            this.InitializeComponent();
        }

        public void Update(OptionType value)
        {
            if (value.IsItemClickEnabled())
            {
                base.Text = value.GetString();
                base.CommandParameter = value;
            }

            if (value.ExistIcon())
            {
                base.Resources.Source = new OptionResourceUri(value);
                base.Tag = new ContentControl
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Template = value.GetTemplate(base.Resources)
                };
            }

            if (value.HasPreview())
            {
                base.KeyboardAcceleratorTextOverride = "•";
            }
            else if (value.HasMenu())
            {
                base.KeyboardAcceleratorTextOverride = ">";
            }
        }
    }
}