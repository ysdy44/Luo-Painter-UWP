using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal sealed class OptionGroupingList : GroupingList<OptionGrouping, OptionType, OptionType> { }
    internal class OptionGrouping : Grouping<OptionType, OptionType> { }

    public sealed partial class OptionIcon : ContentControl
    {

        #region DependencyProperty

        /// <summary> Gets or set the type for <see cref="OptionIcon"/>. </summary>
        public OptionType Type
        {
            get => (OptionType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "OptionIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(OptionType), typeof(OptionIcon), new PropertyMetadata(OptionType.Node, (sender, e) =>
        {
            OptionIcon control = (OptionIcon)sender;

            if (e.NewValue is OptionType value)
            {
                control.Update(value);
            }
        }));

        #endregion

        public OptionIcon()
        {
            this.InitializeComponent();
        }

        private void Update(OptionType value)
        {
            base.Resources.Source = new Uri(value.GetResource());
            base.Template = value.GetTemplate(base.Resources);
        }
    }
}