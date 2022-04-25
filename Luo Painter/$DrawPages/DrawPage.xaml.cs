using Luo_Painter.Blends;
using Luo_Painter.Edits;
using Luo_Painter.Elements;
using Luo_Painter.Historys;
using Luo_Painter.Historys.Models;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Luo_Painter.Shaders;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Luo_Painter
{
    internal static class IconExtensions
    {
        public static Grid GetGrid(UIElement icon, string text) => new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                },
                new ColumnDefinition
                {
                    Width =new GridLength(12)
                },
                new ColumnDefinition
                {
                    Width =new GridLength(1, GridUnitType.Star)
                },
            },
            Children =
            {
                icon,
                text.GetTextBlock().SetColumn(2)
            }
        };
        public static StackPanel GetStackPanel(UIElement icon, string text) => new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                icon,
                new ContentControl
                {
                    Width = 12
                },
                text.GetTextBlock()
            }
        };
        public static TextBlock GetTextBlock(this string text)
        {
            return new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
        }
        public static FrameworkElement SetColumn(this FrameworkElement element, int value)
        {
            Grid.SetColumn(element, value);
            return element;
        }
    }

    internal sealed class ToolIcon : ContentControl
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="ToolIcon"/>. </summary>
        public ToolType Type
        {
            get => (ToolType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ToolIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(ToolType), typeof(ToolIcon), new PropertyMetadata(ToolType.None, (sender, e) =>
        {
            ToolIcon control = (ToolIcon)sender;

            if (e.NewValue is ToolType value)
            {
                control.Content = value;
                control.Template = value.GetTemplate(out ResourceDictionary resource);
                control.Resources = resource;
            }
        }));


        #endregion

        public ToolIcon()
        {
            base.Loaded += (s, e) =>
            {
                ListViewItem parent = SelectedButtonPresenter.FindAncestor(this);
                if (parent is null) return;
                ToolTipService.SetToolTip(parent, new ToolTip
                {
                    Content = this.Type,
                    Style = App.Current.Resources["AppToolTipStyle"] as Style
                });
            };
        }
    }

    internal sealed class BlendIcon : ContentPresenter
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="BlendIcon"/>. </summary>
        public BlendEffectMode Type
        {
            get => (BlendEffectMode)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "BlendIcon.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(BlendEffectMode), typeof(BlendIcon), new PropertyMetadata(BlendEffectMode.Multiply, (sender, e) =>
        {
            BlendIcon control = (BlendIcon)sender;

            if (e.NewValue is BlendEffectMode value)
            {
                control.Content = IconExtensions.GetStackPanel(new ContentControl
                {
                    Content = value,
                    Template = value.GetTemplate(out ResourceDictionary resource, out string title),
                    Resources = resource,
                }, title);
            }
        }));


        #endregion
    }


    internal abstract class OptionButtonBase : Button
    {
        protected abstract void OnTypeChanged(OptionType value);

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="OptionButtonBase"/>. </summary>
        public OptionType Type
        {
            get => (OptionType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "OptionButtonBase.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(OptionType), typeof(OptionButtonBase), new PropertyMetadata(OptionType.None, (sender, e) =>
        {
            OptionButtonBase control = (OptionButtonBase)sender;

            if (e.NewValue is OptionType value)
            {
                control.OnTypeChanged(value);
            }
        }));


        #endregion
    }

    internal sealed class OptionButton : OptionButtonBase
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

    internal sealed class OptionIcon : OptionButtonBase
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = value.ToString();
            base.Template = value.GetTemplate(out ResourceDictionary resource);
            base.Resources = resource;
        }
    }

    internal sealed class OptionItem : OptionButtonBase
    {
        protected override void OnTypeChanged(OptionType value)
        {
            base.Content = IconExtensions.GetStackPanel(new ContentControl
            {
                Content = value,
                Template = value.GetTemplate(out ResourceDictionary resource),
                Resources = resource,
            }, value.ToString());
        }
    }

    internal sealed class EditItem : Button
    {
        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="EditItem"/>. </summary>
        public EditType Type
        {
            get => (EditType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "EditItem.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(EditType), typeof(EditItem), new PropertyMetadata(EditType.None, (sender, e) =>
        {
            EditItem control = (EditItem)sender;

            if (e.NewValue is EditType value)
            {
                control.CommandParameter = value;
                control.Icon = new ContentControl
                {
                    Content = value,
                    Template = value.GetTemplate(out ResourceDictionary resource),
                    Resources = resource,
                };
                control.Icon.GoToState(control.IsEnabled);
                control.Content = IconExtensions.GetGrid(control.Icon, value.ToString());
            }
        }));


        #endregion

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
    }


    internal sealed class ToolGroupingList : GroupingList<ToolGrouping, ToolGroupType, ToolType> { }
    internal sealed class ToolGrouping : Grouping<ToolGroupType, ToolType> { }

    internal sealed class BlendGroupingList : GroupingList<BlendGrouping, BlendGroupType, BlendEffectMode> { }
    internal sealed class BlendGrouping : Grouping<BlendGroupType, BlendEffectMode> { }

    internal sealed class OptionGroupingList : GroupingList<OptionGrouping, OptionGroupType, OptionType> { }
    internal sealed class OptionGrouping : Grouping<OptionGroupType, OptionType> { }

    internal sealed class EditGrouping : Grouping<EditGroupType, EditType> { }


    internal class OptionTypeCommand : RelayCommand<OptionType> { }
    internal class EditTypeCommand : RelayCommand<EditType> { }
    internal class LayerCommand : RelayCommand<ILayer> { }


    internal sealed class RadianRange
    {
        public Range Range { get; } = new Range
        {
            Default = 0,
            Minimum = -180,
            Maximum = 180,
        };
    }

    internal sealed class ScaleRange
    {
        public Range XRange { get; private set; }
        public Range YRange { get; } = new Range
        {
            Default = 1,
            Minimum = 0.1,
            Maximum = 10,
        };
        public InverseProportion InverseProportion { get; } = new InverseProportion
        {
            A = -1,
            B = 10,
            C = 1,
        };
        public ScaleRange() => this.XRange = this.InverseProportion.ConvertYToX(this.YRange);
    }


    public sealed partial class DrawPage : Page
    {

        //@Converter
        private bool ReverseBooleanConverter(bool value) => !value;
        private bool ReverseBooleanConverter(bool? value) => value == false;
        private string RoundConverter(double value) => $"{(value):0}";
        private string Round2Converter(double value) => $"{(value):0.00}";
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        //@Converter
        private string ScaleXToYConverter(double value) => this.Round2Converter(this.ScaleRange.InverseProportion.ConvertXToY(value));

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        //@Converter
        private Symbol ColorSpectrumShapeSymbolConverter(bool? value) => value == true ? Symbol.Target : Symbol.Stop;
        private ColorSpectrumShape ColorSpectrumShapeConverter(bool? value) => value == true ? ColorSpectrumShape.Ring : ColorSpectrumShape.Box;
        private ColorSpectrumComponents ColorSpectrumComponentsConverter(int value)
        {
            switch (value)
            {
                case 0: return ColorSpectrumComponents.SaturationValue;
                case 1: return ColorSpectrumComponents.HueSaturation;
                default: return ColorSpectrumComponents.HueValue;
            }
        }

        Historian<IHistory> History { get; } = new Historian<IHistory>();
        IDictionary<string, ILayer> Layers { get; } = new Dictionary<string, ILayer>();
        ObservableCollection<ILayer> ObservableCollection { get; } = new ObservableCollection<ILayer>();
        BitmapLayer BitmapLayer { get; set; }
        BitmapLayer Clipboard { get; set; }
        BitmapLayer Marquee { get; set; }
        OptionType OptionType { get; set; } = OptionType.None;
        ToolType ToolType { get; set; } = ToolType.PaintBrush;
        ToolGroupType ToolGroupType { get; set; } = ToolGroupType.Paint;

        byte[] LiquefactionShaderCodeBytes;
        byte[] FreeTransformShaderCodeBytes;
        byte[] GradientMappingShaderCodeBytes;
        byte[] RippleEffectShaderCodeBytes;
        byte[] DifferenceShaderCodeBytes;

        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
            this.FreeTransformShaderCodeBytes = await ShaderType.FreeTransform.LoadAsync();
            this.GradientMappingShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
            this.RippleEffectShaderCodeBytes = await ShaderType.RippleEffect.LoadAsync();
            this.DifferenceShaderCodeBytes = await ShaderType.Difference.LoadAsync();
        }

        public DrawPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.ConstructLayers();
            this.ConstructLayer();

            this.ConstructOptions();
            this.ConstructOption();
            this.ConstructTransform();
            this.ConstructGradientMapping();
            this.ConstructRippleEffect();

            this.ConstructTools();
            this.ConstructBlends();
            this.ConstructPaint();

            this.ConstructHistory();

            this.ConstructDialog();
            this.ConstructRadian();
            this.ConstructScale();

            this.ConstructStoryboard();
            this.ConstructSplitStoryboard();

            this.ConstructTip();
            this.ConstructColor();
            this.ConstructColorShape();


            this.ApplicationView.Title = "*Untitled";


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                int count = 0;
                string[] undo = this.ObservableCollection.Select(c => c.Id).ToArray();

                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> items = await e.DataView.GetStorageItemsAsync();
                    foreach (IStorageItem item in items)
                    {
                        if (item is IStorageFile file)
                        {
                            CanvasBitmap bitmap = await this.AddAsync(file);
                            if (bitmap is null) continue;
                            count++;

                            BitmapLayer bitmapLayer = new BitmapLayer(this.CanvasControl, bitmap, this.Transformer.Width, this.Transformer.Height);
                            this.Layers.Add(bitmapLayer.Id, bitmapLayer);
                            this.Add(bitmapLayer);
                        }
                    }
                    this.CanvasControl.Invalidate(); // Invalidate
                }

                if (count == 0) return;
                if (count > 1) this.Tip("Add Images", $"{count}"); // Tip

                // History
                string[] redo = this.ObservableCollection.Select(c => c.Id).ToArray();
                int removes = this.History.Push(new ArrangeHistory(undo, redo));

                this.CanvasControl.Invalidate(); // Invalidate

                this.UndoButton.IsEnabled = this.History.CanUndo;
                this.RedoButton.IsEnabled = this.History.CanRedo;
            };
            base.DragOver += (s, e) =>
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                //e.DragUIOverride.Caption = 
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsContentVisible = e.DragUIOverride.IsGlyphVisible = true;
            };
        }
    }
}