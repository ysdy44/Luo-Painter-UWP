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
    internal sealed class OptionItem : TButton<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.CommandParameter = value;
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }

    internal sealed class OptionImage : TButton<OptionType>
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

    internal sealed class OptionIcon : TButton<OptionType>
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