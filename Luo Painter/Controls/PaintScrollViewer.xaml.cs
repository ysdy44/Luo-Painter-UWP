using Luo_Painter.Brushes;
using Luo_Painter.UI;
using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Luo_Painter.Controls
{
    [ContentProperty(Name = nameof(Child))]
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter, IInkSlider
    {
        //@Delegate
        public event RoutedEventHandler ScratchpadClick { remove => this.ScratchpadButton.Click -= value; add => this.ScratchpadButton.Click += value; }
        public event EventHandler<object> Opened;
        public event EventHandler<object> Closed;

        //@Converter
        private double PercentageConverter(double value) => System.Math.Clamp(value / 100d, 0d, 1d);
        private CornerRadius CornerRadiusConverter(int value) => new CornerRadius((value is 0 || value is 1) ? 40 : 0);
        private double TipOpacityConverter(int value) => (value is 0 || value is 2) ? 1 : 0;
        private int ZeroConverter(InkType value) => 0;

        #region Converter

        private Visibility SpacingVisibilityConverter(InkType value) => value.HasFlag(InkType.UISpacing) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility FlowVisibilityConverter(InkType value) => value.HasFlag(InkType.UIFlow) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility HardnessVisibilityConverter(InkType value) => value.HasFlag(InkType.UIHardness) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility TipVisibilityConverter(InkType value) => value.HasFlag(InkType.UITip) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility TextureVisibilityConverter(InkType value) => value.HasFlag(InkType.UIBlendMode) || value.HasFlag(InkType.UIGrain) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ShapeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIShape) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility BlendModeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIBlendMode) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility GrainVisibilityConverter(InkType value) => value.HasFlag(InkType.UIGrain) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility MixVisibilityConverter(InkType value) => value.HasFlag(InkType.UIMix) ? Visibility.Visible : Visibility.Collapsed;

        #endregion

        public CanvasDevice CanvasDevice => this.InkParameter.CanvasDevice;
        public object Child { get => this.ContentPresenter.Content; set => this.ContentPresenter.Content = value; }
        public bool IsInkEnabled { get; set; } = true;

        readonly PressurePoints SizePressurePoints = new PressurePoints();
        readonly PressurePoints FlowPressurePoints = new PressurePoints();

        double StatringX;

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
        public void TryInkAsync() => this.InkParameter.TryInkAsync();
        public void TryInk() => this.InkParameter.TryInk();

        #endregion

        private readonly InkType[] Inks = new InkType[]
        {
            InkType.Erase,
            InkType.General,
            InkType.Tip,
            InkType.Line,
            InkType.Blur,
            InkType.Mosaic,
            InkType.Liquefy,
        };

        //@Construct
        public PaintScrollViewer()
        {
            this.InitializeComponent();
            this.ConstructPicker();

            this.ConstructProperty();
            this.ConstructTexture();
            this.ConstructMix();

            this.IsDrak = base.ActualTheme is ElementTheme.Dark;
            base.ActualThemeChanged += (s, e) =>
            {
                this.IsDrak = base.ActualTheme is ElementTheme.Dark;
                this.TryInkAsync();
            };
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.DoubleAnimation.To = e.NewSize.Height;
            };

            this.HideStoryboard.Completed += this.Closed;
            this.ShowStoryboard.Completed += this.Opened;

            this.Opened += (s, e) => this.ConstructInkSliderValue(this.InkPresenter);

            this.Thumb.DragStarted += (s, e) => this.StatringX = this.TranslateTransform.Y;
            this.Thumb.DragDelta += (s, e) => this.TranslateTransform.Y = System.Math.Max(0, this.StatringX += e.VerticalChange);
            this.Thumb.DragCompleted += (s, e) => this.Toggle(this.StatringX < base.ActualHeight / 2);
        }

        private void HideCore()
        {
            this.RootGrid.Visibility = Visibility.Collapsed;
            this.RootGrid.IsHitTestVisible = false;

            //this.TranslateTransform.Y = base.ActualHeight;
            this.TranslateTransform.Y = 0;

            this.Closed?.Invoke(this, null); // Delegate
        }

        private void ShowCore()
        {
            this.RootGrid.Visibility = Visibility.Visible;
            this.RootGrid.IsHitTestVisible = true;

            this.TranslateTransform.Y = 0;

            this.Opened?.Invoke(this, null); // Delegate
        }

        public void Hide()
        {
            if (App.UISettings.AnimationsEnabled)
            {
                this.HideStoryboard.Begin();
            }
            else
            {
                this.HideCore();
            }
        }

        public void Show()
        {
            if (App.UISettings.AnimationsEnabled)
            {
                this.ShowStoryboard.Begin();
            }
            else
            {
                this.ShowCore();
            }
        }

        public void Toggle() => this.Toggle(base.Visibility != default);
        private void Toggle(bool isShow)
        {
            if (App.UISettings.AnimationsEnabled)
            {
                if (isShow)
                {
                    this.HideStoryboard.Pause();
                    this.ShowStoryboard.Begin();
                }
                else
                {
                    this.ShowStoryboard.Pause();
                    this.HideStoryboard.Begin();
                }
            }
            else
            {
                if (isShow)
                {
                    this.ShowCore();
                }
                else
                {
                    this.HideCore();
                }
            }
        }
    }
}