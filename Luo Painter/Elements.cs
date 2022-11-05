using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Options;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Luo_Painter.Projects;

namespace Luo_Painter
{
    internal sealed class MainPageToDrawPageAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public MainPageToDrawPageAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(MainPage)} to {typeof(DrawPage)}, Parameter is {typeof(ProjectParameter)}, NavigationMode is {this.NavigationMode}";
    }
    internal sealed class DrawPageToStylePageAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public DrawPageToStylePageAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(DrawPage)} to {typeof(StylePage)}, NavigationMode is {this.NavigationMode}";
    }

    public class XamlListView : ListView
    {
        // 1. It is a UserControl.xaml that contains a ListView.
        // <UserControl>
        //      <ListView>
        //      ...
        //      </ListView>
        // </UserControl>
        // Ok.

        // 2. It is a UserControl.xaml, RootNode is ListView.
        // <ListView>
        //      ...
        // </ListView>
        // Exception:
        // Windows.UI.Xaml.Markup.XamlParseException:
        // “XAML parsing failed.”
        // Why ?

        // 3. It is a UserControl.xaml, RootNode is XamlListView.
        // <local:XamlListView>
        //      ...
        // </local:XamlListView>
        // Ok, but why ?
    }

    internal sealed class SizeRange : InverseProportionRange
    {
        public SizeRange() : base(12, 1, 400, 100000) { }
    }
    internal sealed class SpacingRange : InverseProportionRange
    {
        public SpacingRange() : base(25, 10, 400, 1000000) { }
    }
    internal sealed class ScaleRange : InverseProportionRange
    {
        public ScaleRange() : base(1, 0.1, 10, 100) { }
    }

    internal sealed class AdjustmentGroupingList : GroupingList<AdjustmentGrouping, OptionType, OptionType> { }
    internal sealed class AdjustmentGrouping : Grouping<OptionType, OptionType> { }
    internal sealed class AdjustmentImage : TIcon<OptionType>
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

    internal sealed class ElementIcon : TIcon<ElementType>
    {
        protected override void OnTypeChanged(ElementType value)
        {
            base.Width = double.NaN;
            base.VerticalContentAlignment = VerticalAlignment.Center;
            base.HorizontalContentAlignment = HorizontalAlignment.Center;
            base.Content = value;
            base.Resources.Source = new Uri(value.GetResource());
            base.Template = value.GetTemplate(base.Resources);
        }
    }
    internal sealed class ElementItem : TIcon<ElementType>
    {
        protected override void OnTypeChanged(ElementType value)
        {
            base.Resources.Source = new Uri(value.GetResource());
            base.Content = Element.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(base.Resources),
            }, value.ToString());
        }
    }

    internal sealed class ToolGroupingList : GroupingList<ToolGrouping, OptionType, OptionType> { }
    internal class ToolGrouping : Grouping<OptionType, OptionType> { }
    internal sealed class ToolIcon : TIcon<OptionType>
    {
        public ToolIcon()
        {
            base.Loaded += (s, e) =>
            {
                ListViewItem parent = this.FindAncestor<ListViewItem>();
                if (parent is null) return;
                ToolTipService.SetToolTip(parent, new ToolTip
                {
                    Content = this.Type,
                    Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            };
        }
        protected override void OnTypeChanged(OptionType value)
        {
            base.Width = 32;
            base.VerticalContentAlignment = VerticalAlignment.Center;
            base.HorizontalContentAlignment = HorizontalAlignment.Center;
            base.Content = value;
            base.Resources.Source = new Uri(value.GetResource());
            base.Template = value.GetTemplate(base.Resources);
        }
    }

    internal sealed class InkList : List<InkType> { }
    internal sealed class InkIcon : TIcon<InkType>
    {
        protected override void OnTypeChanged(InkType value)
        {
            base.Width = 32;
            base.VerticalContentAlignment = VerticalAlignment.Center;
            base.HorizontalContentAlignment = HorizontalAlignment.Center;
            base.Content = value;
            base.Resources.Source = new Uri(value.GetResource());
            base.Template = value.GetTemplate(base.Resources);
        }
    }
    internal sealed class InkItem : TIcon<InkType>
    {
        protected override void OnTypeChanged(InkType value)
        {
            base.Resources.Source = new Uri(value.GetResource());
            base.Content = Element.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(base.Resources),
            }, value.ToString());
        }
    }

    internal sealed class BlendList : List<BlendEffectMode> { }
    internal sealed class BlendGroupingList : List<BlendEffectMode> { }
    internal sealed class BlendIcon : TIcon<BlendEffectMode>
    {
        protected override void OnTypeChanged(BlendEffectMode value)
        {
            base.Content = value.GetTitle();
        }
    }

    internal class OptionItemButtonWithIsEnabled : OptionItemButton
    {
        public OptionItemButtonWithIsEnabled()
        {
            base.Loaded += (s, e) => this.ContentControl.GoToState(base.IsEnabled);
            base.IsEnabledChanged += (s, e) =>
            {
                if (e.NewValue is bool value)
                {
                    this.ContentControl.GoToState(value);
                }
            };
        }
    }

    internal class OptionItem : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Resources.Source = new Uri(value.GetResource());
            base.Content = Element.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(base.Resources),
            }, value.ToString());
        }
    }
    internal class OptionItemButton : TButton<OptionType>
    {
        protected ContentControl ContentControl = new ContentControl
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        protected override void OnTypeChanged(OptionType value)
        {
            base.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            base.CommandParameter = value;
            base.Resources.Source = new Uri(value.GetResource());
            this.ContentControl.Template = value.GetTemplate(base.Resources);

            if (value.HasPreview())
            {
                base.Content = Element.GetGrid(this.ContentControl, value.ToString());
            }
            else
            {
                this.ContentControl.Width = 32;
                base.Content = Element.GetStackPanel(this.ContentControl, value.ToString());
            }
        }
    }

    [ContentProperty(Name = nameof(Content))]
    internal sealed class OptionItemCase : OptionItem, ICase<OptionType>
    {
        public OptionType Value => base.Type;
        public void OnNavigatedFrom() { }
        public void OnNavigatedTo() { }
    }
    [ContentProperty(Name = nameof(Content))]
    internal class OptionCase : DependencyObject, ICase<OptionType>
    {
        public object Content
        {
            get => (object)base.GetValue(ContentProperty);
            set => base.SetValue(ContentProperty, value);
        }
        /// <summary> Identifies the <see cref="Content"/> property. </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(OptionCase), new PropertyMetadata(null));

        public OptionType Value
        {
            get => (OptionType)base.GetValue(ValueProperty);
            set => base.SetValue(ValueProperty, value);
        }
        /// <summary> Identifies the <see cref="Value"/> property. </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(OptionType), typeof(OptionCase), new PropertyMetadata(default(OptionType)));

        public void OnNavigatedTo() { }

        public void OnNavigatedFrom() { }
    }
    [ContentProperty(Name = nameof(SwitchCases))]
    internal sealed class OptionSwitchPresenter : SwitchPresenter<OptionType> { }

    internal class OptionKeyboardAccelerator : KeyboardAccelerator
    {
        public OptionType CommandParameter { get; set; }
        public OptionTypeCommand Command { get; set; }
        public OptionKeyboardAccelerator() => base.Invoked += (s, e) => this.Command.Execute(this.CommandParameter);
        public override string ToString() => this.CommandParameter.ToString();
    }
    internal class OptionTypeCommand : RelayCommand<OptionType> { }

    internal static class Element
    {

        //@Static
        public static Grid GetGrid(UIElement icon, string text) => new Grid
        {
            ColumnSpacing = 12,
            ColumnDefinitions =
            {
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                },
                new ColumnDefinition
                {
                    Width =new GridLength(1, GridUnitType.Star)
                },
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                },
            },
            Children =
            {
                icon,
                Element.GetTextBlock(text, 1),
                Element.GetFontIcon(2),
            }
        };

        public static StackPanel GetStackPanel(UIElement icon, string text) => new StackPanel
        {
            Spacing = 12,
            Orientation = Orientation.Horizontal,
            Children =
            {
                icon,
                Element.GetTextBlock(text)
            }
        };

        public static FrameworkElement GetTextBlock(string text, int column = 0)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            if (column != 0) Grid.SetColumn(textBlock, column);
            return textBlock;
        }

        public static FrameworkElement GetFontIcon(int column = 0)
        {
            FontIcon fontIcon = new FontIcon
            {
                FontSize = 12,
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = "\uE00F"
            };
            if (column != 0) Grid.SetColumn(fontIcon, column);
            return fontIcon;
        }

    }
}