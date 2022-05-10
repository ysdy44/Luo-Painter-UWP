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
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

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
            this.ConstructSimulater();
            
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
            this.ExportPlacementTarget.Click += (s, e) => this.ExportMenu.Toggle(this.ExportPlacementTarget, ExpanderPlacementMode.Bottom);
            this.ToolPlacementTarget.Click += (s, e) => this.ToolMenu.Toggle(this.ToolPlacementTarget, ExpanderPlacementMode.Bottom);
            this.EditPlacementTarget.Click += (s, e) => this.EditMenu.Toggle(this.EditPlacementTarget, ExpanderPlacementMode.Bottom);
            this.OptionPlacementTarget.Click += (s, e) => this.OptionMenu.Toggle(this.OptionPlacementTarget, ExpanderPlacementMode.Bottom);
            this.MoreOptionPlacementTarget.Click += (s, e) => this.MoreOptionMenu.Toggle(this.MoreOptionPlacementTarget, ExpanderPlacementMode.Bottom);
            this.SetupPlacementTarget.Click += (s, e) => this.SetupMenu.Toggle(this.SetupPlacementTarget, ExpanderPlacementMode.Bottom);


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