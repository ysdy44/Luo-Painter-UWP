using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : ContentDialog, IInkParameter
    {

        //@Converter
        private int IntConverter(double value) => (int)value;
        private string RoundConverter(double value) => $"{value:0}";
        private int SizeXToYConverter(double value) => (int)this.SizeRange.ConvertXToY(value);
        private int SpacingXToYConverter(double value) => (int)this.SpacingRange.ConvertXToY(value);

        private bool BooleanConverter(bool? value) => value is true;
        private double PercentageConverter(double value) => System.Math.Clamp(value / 100d, 0d, 1d);
        private double OpacityConverter(bool value) => value ? 1 : 0.5;

        private Visibility BooleanToVisibilityConverter(bool? value) => value is true ? Visibility.Visible : Visibility.Collapsed;
        private CornerRadius CornerRadiusConverter(int value) => new CornerRadius((value is 0 || value is 1) ? 40 : 0);
        private double TipOpacityConverter(int value) => (value is 0 || value is 2) ? 1 : 0;

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

            this.ConstructPicker();

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