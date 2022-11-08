using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers;
using Luo_Painter.Layers.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        //@Converter
        private string RoundConverter(double value) => $"{value:0}";
        private string SizeXToYConverter(double value) => this.RoundConverter(this.SizeRange.ConvertXToY(value));
        private string SpacingXToYConverter(double value) => this.RoundConverter(this.SpacingRange.ConvertXToY(value));

        private bool BooleanConverter(bool? value) => value is true;
        private double PercentageConverter(double value) => System.Math.Clamp(value / 100d, 0d, 1d);
        private double OpacityConverter(bool value) => value ? 1 : 0.5;

        private Visibility BooleanToVisibilityConverter(bool? value) => value is true ? Visibility.Visible : Visibility.Collapsed;
        private CornerRadius CornerRadiusConverter(int value) => new CornerRadius((value is 0 || value is 1) ? 40 : 0);
        private double TipOpacityConverter(int value) => (value is 0 || value is 2) ? 1 : 0;

        private Visibility Int0VisibilityConverter(int value) => value is 0 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Int1VisibilityConverter(int value) => value is 1 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Int2VisibilityConverter(int value) => value is 2 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Int3VisibilityConverter(int value) => value is 3 ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Int4VisibilityConverter(int value) => value is 4 ? Visibility.Visible : Visibility.Collapsed;

        private Visibility SpacingVisibilityConverter(InkType value) => value.HasFlag(InkType.UISpacing) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility FlowVisibilityConverter(InkType value) => value.HasFlag(InkType.UIFlow) ? Visibility.Visible : Visibility.Collapsed;

        private Visibility HardnessVisibilityConverter(InkType value) => value.HasFlag(InkType.UIHardness) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility TipVisibilityConverter(InkType value) => value.HasFlag(InkType.UITip) ? Visibility.Visible : Visibility.Collapsed;

        private Visibility ShapeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIShape) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility GrainVisibilityConverter(InkType value) => value.HasFlag(InkType.UIGrain) ? Visibility.Visible : Visibility.Collapsed;

        private Visibility BlendModeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIBlendMode) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility MixColorVisibilityConverter(InkType value) => value.HasFlag(InkType.UIMix) ? Visibility.Visible : Visibility.Collapsed;


        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;

        //@Task
        readonly object InkLocker = new object();
        CanvasRenderTarget InkRender { get; set; }

        bool InkIsEnabled = true;

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="PaintScrollViewer"/>. </summary>
        public InkType Type
        {
            get => (InkType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "PaintScrollViewer.Type" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(InkType), typeof(PaintScrollViewer), new PropertyMetadata(InkType.Tip));


        #endregion

        #region IInkParameter

        public InkType InkType { get => this.InkParameter.InkType; set => this.InkParameter.InkType = value; }
        public InkPresenter InkPresenter => this.InkParameter.InkPresenter;

        bool IsDrak;
        public Color Color => this.IsDrak ? Colors.White : Colors.Black;
        public Vector4 ColorHdr => this.IsDrak ? Vector4.One : Vector4.UnitW;

        public string TextureSelectedItem => this.InkParameter.TextureSelectedItem;
        public void ConstructTexture(string path) => this.InkParameter.ConstructTexture(path);
        public Task<ContentDialogResult> ShowTextureAsync() => this.InkParameter.ShowTextureAsync();

        IInkParameter InkParameter;
        public void Construct(IInkParameter item)
        {
            this.InkParameter = item;

            this.ConstructInk(item.InkPresenter);
            this.Type = item.InkPresenter.Type;
        }

        #endregion

        //@Construct
        public PaintScrollViewer()
        {
            this.InitializeComponent();
            this.ConstructCanvas();

            this.ConstructInk1();
            this.ConstructInk2();
            this.ConstructInk3();

            this.IsDrak = base.ActualTheme is ElementTheme.Dark;
            base.ActualThemeChanged += (s, e) =>
            {
                this.IsDrak = base.ActualTheme is ElementTheme.Dark;
                this.TryInkAsync();
            };
        }

    }
}