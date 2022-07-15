﻿using Luo_Painter.Blends;
using Luo_Painter.Brushes;
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


    internal sealed class BlendIcon : TIcon<BlendEffectMode>
    {
        protected override void OnTypeChanged(BlendEffectMode value)
        {
            base.Content = value.GetTitle();
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


    public sealed partial class DrawPage : Page, ILayerManager
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
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;
        private Symbol FlowDirectionToSymbolConverter(FlowDirection value) => value is FlowDirection.LeftToRight ? Symbol.Back : Symbol.Forward;

        //@Converter
        private Vector2 ToPosition(Vector2 point) => Vector2.Transform(this.CanvasVirtualControl.Dpi.ConvertDipsToPixels(point), this.Transformer.GetInverseMatrix());
        private Vector2 ToPoint(Vector2 position) => this.CanvasVirtualControl.Dpi.ConvertPixelsToDips(Vector2.Transform(position, this.Transformer.GetMatrix()));

        CanvasDevice CanvasDevice { get; } = new CanvasDevice();
        Historian<IHistory> History { get; } = new Historian<IHistory>();

        public LayerDictionary Layers { get; } = new LayerDictionary();
        public LayerNodes Nodes { get; } = new LayerNodes();
        public LayerObservableCollection ObservableCollection { get; } = new LayerObservableCollection();
        public IList<string> ClipboardLayers { get; } = new List<string>();

        public int LayerSelectedIndex { get => this.LayerListView.SelectedIndex; set => this.LayerListView.SelectedIndex = value; }
        public object LayerSelectedItem { get => this.LayerListView.SelectedItem; set => this.LayerListView.SelectedItem = value; }
        public IList<object> LayerSelectedItems => this.LayerListView.SelectedItems;

        InkMixer InkMixer { get; set; } = new InkMixer();
        InkPresenter InkPresenter { get; } = new InkPresenter();
        InkRender InkRender { get; set; }

        Mesh Mesh { get; set; }
        GradientMesh GradientMesh { get; set; }
        BitmapLayer BitmapLayer { get; set; }
        BitmapLayer Clipboard { get; set; }
        BitmapLayer Marquee { get; set; }
        BitmapLayer Displacement { get; set; }

        bool IsFullScreen { get; set; }
        SelectionType SelectionType { get; set; } = SelectionType.None;
        OptionType OptionType { get; set; } = OptionType.PaintBrush;
        InkType InkType { get; set; } = InkType.None;

        byte[] LiquefactionShaderCodeBytes;
        byte[] FreeTransformShaderCodeBytes;
        byte[] GradientMappingShaderCodeBytes;
        byte[] RippleEffectShaderCodeBytes;
        byte[] DifferenceShaderCodeBytes;
        byte[] DottedLineTransformShaderCodeBytes;
        byte[] LalphaMaskShaderCodeBytes;
        byte[] RalphaMaskShaderCodeBytes;
        byte[] DisplacementLiquefactionShaderCodeBytes;
        byte[] LalphaMaskEffectShaderCodeBytes;
        byte[] RalphaMaskEffectShaderCodeBytes;

        byte[] BrushEdgeHardnessShaderCodeBytes;
        byte[] BrushEdgeHardnessWithTextureShaderCodeBytes;

        private async Task CreateResourcesAsync()
        {
            this.LiquefactionShaderCodeBytes = await ShaderType.Liquefaction.LoadAsync();
            this.FreeTransformShaderCodeBytes = await ShaderType.FreeTransform.LoadAsync();
            this.GradientMappingShaderCodeBytes = await ShaderType.GradientMapping.LoadAsync();
            this.RippleEffectShaderCodeBytes = await ShaderType.RippleEffect.LoadAsync();
            this.DifferenceShaderCodeBytes = await ShaderType.Difference.LoadAsync();
            this.LalphaMaskShaderCodeBytes = await ShaderType.LalphaMask.LoadAsync();
            this.RalphaMaskShaderCodeBytes = await ShaderType.RalphaMask.LoadAsync();
            this.DisplacementLiquefactionShaderCodeBytes = await ShaderType.DisplacementLiquefaction.LoadAsync();
            this.LalphaMaskEffectShaderCodeBytes = await ShaderType.LalphaMaskEffect.LoadAsync();
            this.RalphaMaskEffectShaderCodeBytes = await ShaderType.RalphaMaskEffect.LoadAsync();

            // Brush
            this.BrushEdgeHardnessShaderCodeBytes = await ShaderType.BrushEdgeHardness.LoadAsync();
            this.BrushEdgeHardnessWithTextureShaderCodeBytes = await ShaderType.BrushEdgeHardnessWithTexture.LoadAsync();
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

            this.ConstructFoots();
            this.ConstructFoot();

            this.ConstructBrush();
            this.ConstructInk();

            this.ConstructEdits();
            this.ConstructSetup();

            this.ConstructOptions();
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

            this.LightDismissOverlay.Tapped += (s, e) => this.ExpanderLightDismissOverlay.Hide();
            this.ExpanderLightDismissOverlay.IsFlyoutChanged += (s, isFlyout) => this.LightDismissOverlay.Visibility = isFlyout ? Visibility.Visible : Visibility.Collapsed;

            this.ExportButton.Click += (s, e) => this.ExportMenu.Toggle(this.ExportButton, ExpanderPlacementMode.Bottom);

            this.ToolButton.Click += (s, e) => this.ToolMenu.Toggle(this.ToolButton, ExpanderPlacementMode.Bottom);
            this.PaintButton.Click += (s, e) => this.PaintMenu.Toggle(this.PaintButton, ExpanderPlacementMode.Bottom);
            this.BrushButton.Click += (s, e) => this.BrushMenu.Toggle(this.BrushButton, ExpanderPlacementMode.Bottom);
            this.SizeButton.Click += (s, e) => this.SizeMenu.Toggle(this.SizeButton, ExpanderPlacementMode.Bottom);
            this.HistoryButton.Click += (s, e) => this.HistoryMenu.Toggle(this.HistoryButton, ExpanderPlacementMode.Bottom);
            this.ColorButton.Click += (s, e) => this.ColorMenu.Toggle(this.ColorButton, ExpanderPlacementMode.Bottom);

            this.EditButton.Click += (s, e) => this.EditMenu.Toggle(this.EditButton, ExpanderPlacementMode.Bottom);
            this.AdjustmentButton.Click += (s, e) => this.AdjustmentMenu.Toggle(this.AdjustmentButton, ExpanderPlacementMode.Bottom);
            this.OtherButton.Click += (s, e) => this.OtherMenu.Toggle(this.OtherButton, ExpanderPlacementMode.Bottom);
            
            this.LayerListView.Add += (s, e) => this.AddMenu.Toggle(this.LayerListView.PlacementTarget, ExpanderPlacementMode.Left);
            this.LayerListView.Remove += (s, e) => this.Edit(OptionType.Remove);
            this.LayerListView.Opening += (s, e) => this.LayerMenu.Toggle(this.LayerListView.PlacementTarget, ExpanderPlacementMode.Left);


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