using Luo_Painter.Elements;
using Luo_Painter.Options;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter.Controls
{
    internal abstract class OptionItemBase : Button
    {
        protected abstract void OnTypeChanged(OptionType value);

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="OptionItemBase"/>. </summary>
        public OptionType Type
        {
            get => (OptionType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "OptionItemBase.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(OptionType), typeof(OptionItemBase), new PropertyMetadata(OptionType.None, (sender, e) =>
        {
            OptionItemBase control = (OptionItemBase)sender;

            if (e.NewValue is OptionType value)
            {
                control.OnTypeChanged(value);
            }
        }));


        #endregion
    }

    internal sealed class OptionItem : OptionItemBase
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.CommandParameter = value;
            base.Content = IconExtensions.GetStackPanel(new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }

    internal sealed class OptionImage : OptionItemBase
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = value.ToString();
            // https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media
            {
                BitmapImage bitmap = new BitmapImage();
                base.Background = new ImageBrush
                {
                    ImageSource = bitmap
                };
                bitmap.UriSource = new Uri(value.GetThumbnail());
            }
        }
    }

    internal sealed class OptionIcon : OptionItemBase
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = value.ToString();
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

    internal sealed class OptionGroupingList : GroupingList<OptionGrouping, OptionGroupType, OptionType> { }
    internal sealed class OptionGrouping : Grouping<OptionGroupType, OptionType> { }

    internal class OptionTypeCommand : RelayCommand<OptionType> { }

    public sealed partial class OptionButton : Button
    {
        //@Delegate
        public event EventHandler<OptionType> ItemClick
        {
            remove => this.OptionTypeCommand.Click -= value;
            add => this.OptionTypeCommand.Click += value;
        }

        //@Construct
        public OptionButton()
        {
            this.InitializeComponent();
            this.ItemClick += (s, e) => this.OptionFlyout.Hide();
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

    }
}