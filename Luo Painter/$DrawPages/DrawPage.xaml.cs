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
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
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


    [ContentProperty(Name = nameof(Content))]
    internal class OptionCase : Case<OptionType> { }

    [ContentProperty(Name = nameof(SwitchCases))]
    internal class OptionSwitchPresenter : SwitchPresenter<OptionType> { }


    [ContentProperty(Name = nameof(Content))]
    internal class ToolGroupCase : GroupCase<ToolGroupType, ToolType> { }

    [ContentProperty(Name = nameof(SwitchCases))]
    internal class ToolGroupSwitchPresenter : SwitchGroupPresenter<ToolGroupType, ToolType> { }


    public sealed partial class DrawPage : Page
    {

        //@Key
        private bool IsKeyDown(VirtualKey key) => Window.Current.CoreWindow.GetKeyState(key).HasFlag(CoreVirtualKeyStates.Down);
        private bool IsCtrl => this.IsKeyDown(VirtualKey.Control);
        private bool IsShift => this.IsKeyDown(VirtualKey.Shift);
        private bool IsAlt => this.IsKeyDown(VirtualKey.Menu);
        private bool IsSpace => this.IsKeyDown(VirtualKey.Space);

        //@Converter
        private bool ReverseBooleanConverter(bool value) => !value;
        private bool ReverseBooleanConverter(bool? value) => value == false;
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        Historian<IHistory> History { get; } = new Historian<IHistory>();
        IDictionary<string, ILayer> Layers { get; } = new Dictionary<string, ILayer>();
        ObservableCollection<ILayer> ObservableCollection { get; } = new ObservableCollection<ILayer>();
        BitmapLayer BitmapLayer { get; set; }
        BitmapLayer Clipboard { get; set; }
        BitmapLayer Marquee { get; set; }
        bool IsFullScreen { get; set; }
        OptionType OptionType { get; set; } = OptionType.None;
        ToolType ToolType { get; set; } = ToolType.PaintBrush;

        byte[] LiquefactionShaderCodeBytes;
        byte[] FreeTransformShaderCodeBytes;
        byte[] GradientMappingShaderCodeBytes;
        byte[] RippleEffectShaderCodeBytes;
        byte[] DifferenceShaderCodeBytes;
        byte[] DottedLineTransformShaderCodeBytes;

        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
            this.FreeTransformShaderCodeBytes = await ShaderType.FreeTransform.LoadAsync();
            this.GradientMappingShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
            this.RippleEffectShaderCodeBytes = await ShaderType.RippleEffect.LoadAsync();
            this.DifferenceShaderCodeBytes = await ShaderType.Difference.LoadAsync();
        }
        private async Task CreateDottedLineResourcesAsync()
        {
            this.DottedLineTransformShaderCodeBytes = await ShaderType.DottedLineTransform.LoadAsync();
        }

        public DrawPage()
        {
            this.InitializeComponent();
            this.ConstructCanvas();
            this.ConstructOperator();

            this.ConstructLayers();
            this.ConstructLayer();

            this.ConstructEdits();

            this.ConstructOptions();
            this.ConstructOption();
            this.ConstructTransform();
            this.ConstructGradientMapping();
            this.ConstructRippleEffect();

            this.ConstructTools();
            this.ConstructMarquee();
            this.ConstructVector();

            this.ConstructHistory();

            this.ConstructDialog();
            this.ConstructColor();
            this.ConstructStoryboard();


            this.ApplicationView.Title = "*Untitled";


            // Drag and Drop 
            base.AllowDrop = true;
            base.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    this.AddAsync(from file in await e.DataView.GetStorageItemsAsync() where file is IStorageFile select file as IStorageFile);
                }
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