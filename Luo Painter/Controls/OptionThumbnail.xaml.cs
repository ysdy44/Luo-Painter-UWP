using Luo_Painter.Options;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class OptionThumbnail : UserControl
    {

        #region DependencyProperty

        /// <summary> Gets or set the type for <see cref="OptionThumbnail"/>. </summary>
        public OptionType Type
        {
            get => (OptionType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "OptionThumbnail.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(OptionType), typeof(OptionThumbnail), new PropertyMetadata(OptionType.Node, (sender, e) =>
        {
            OptionThumbnail control = (OptionThumbnail)sender;

            if (e.NewValue is OptionType value)
            {
                control.Update(value);
            }
        }));

        #endregion

        public OptionThumbnail()
        {
            this.InitializeComponent();
        }

        private void Update(OptionType value)
        {
            if (value.ExistThumbnail())
            {
                this.TextBlock.Text = App.Resource.GetString(value.ToString());
                this.BitmapImage.UriSource = new Uri(value.GetThumbnail());
            }
        }
    }
}