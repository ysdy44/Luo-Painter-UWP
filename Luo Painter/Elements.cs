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
    internal sealed class DrawPageToBrushPageAttribute : Attribute
    {
        readonly NavigationMode NavigationMode;
        public DrawPageToBrushPageAttribute(NavigationMode navigationMode) => this.NavigationMode = navigationMode;
        public override string ToString() => $"{typeof(DrawPage)} to {typeof(BrushPage)}, Parameter is {typeof(IInkParameter)}, NavigationMode is {this.NavigationMode}";
    }

    internal sealed class SizeRange : InverseProportionRange
    {
        public SizeRange() : base(12, 1, 400, 100000) { }
    }
    internal sealed class SpacingRange : InverseProportionRange
    {
        public SpacingRange() : base(25, 10, 400, 1000000) { }
    }

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

    internal sealed class AdjustmentGroupingList : GroupingList<AdjustmentGrouping, OptionType, OptionType> { }
    internal sealed class AdjustmentGrouping : Grouping<OptionType, OptionType> { }
    internal sealed class AdjustmentImage : TButton<OptionType>
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

    internal sealed class BrushGroupingList : List<BrushGrouping> { }
    internal class BrushGrouping : List<PaintBrush>, IList<PaintBrush>, IGrouping<PaintBrushGroupType, PaintBrush>
    {
        public PaintBrushGroupType Key { set; get; }
    }
    internal class BrushCommand : RelayCommand<PaintBrush> { }

    internal sealed class ElementIcon : TIcon<ElementType>
    {
        protected override void OnTypeChanged(ElementType value)
        {
            base.Width = double.NaN;
            base.VerticalContentAlignment = VerticalAlignment.Center;
            base.HorizontalContentAlignment = HorizontalAlignment.Center;
            base.Content = value;
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }
    internal sealed class ElementItem : TIcon<ElementType>
    {
        protected override void OnTypeChanged(ElementType value)
        {
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }

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
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }
    internal sealed class ToolItem : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }
    internal sealed class ToolGroupingList : List<ToolGrouping> { }
    internal class ToolGrouping : List<OptionType>, IList<OptionType>, IGrouping<OptionType, OptionType>
    {
        public OptionType Key { set; get; }
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

    internal sealed class OptionIcon : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = value.ToString();
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }
    internal sealed class OptionItem : TButton<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.CommandParameter = value;
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }

    internal class OptionItemIcon : TIcon<OptionType>
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = TIconExtensions.GetStackPanel(new ContentControl
            {
                Width = 32,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }
    [ContentProperty(Name = nameof(Content))]
    internal sealed class OptionItemIconCase : OptionItemIcon, ICase<OptionType>
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
}