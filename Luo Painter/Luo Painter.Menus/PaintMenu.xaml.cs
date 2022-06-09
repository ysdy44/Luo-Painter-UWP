using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Luo_Painter.Layers.Models;
using Luo_Painter.Options;
using Luo_Painter.Tools;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;

namespace Luo_Painter.Menus
{
    internal sealed class SizeRange : InverseProportionRange
    {
        public SizeRange() : base(12, 1, 400, 100000) { }
    }

    internal sealed class SpacingRange : InverseProportionRange
    {
        public SpacingRange() : base(25, 10, 400, 1000000) { }
    }

    public sealed partial class PaintMenu : Expander
    {
        //@Delegate
        public event RoutedEventHandler SelectMask { remove => this.SelectMaskButton.Click -= value; add => this.SelectMaskButton.Click += value; }
        public event RoutedEventHandler SelectPattern { remove => this.SelectPatternButton.Click -= value; add => this.SelectPatternButton.Click += value; }

        public event RoutedEventHandler MaskClosed;
        public event RoutedEventHandler PatternClosed;

        public event RoutedEventHandler MaskOpened;
        public event RoutedEventHandler PatternOpened;

        //@Converter
        private string RoundConverter(double value) => $"{value:0}";
        private string SizeXToYConverter(double value) => this.RoundConverter(this.SizeRange.ConvertXToY(value));
        private string SpacingXToYConverter(double value) => this.RoundConverter(this.SpacingRange.ConvertXToY(value));
        private Visibility Int0ToVisibility(OptionType value) => value == OptionType.PaintBrush ? Visibility.Visible : Visibility.Collapsed;
        private Visibility Int012ToVisibility(OptionType value)
        {
            switch (value)
            {
                case OptionType.PaintBrush: case OptionType.PaintWatercolorPen: case OptionType.PaintPencil: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }
        private Visibility Int013ToVisibility(OptionType value)
        {
            switch (value)
            {
                case OptionType.PaintBrush: case OptionType.PaintWatercolorPen: case OptionType.PaintEraseBrush: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }
        private Visibility Int0123ToVisibility(OptionType value)
        {
            switch (value)
            {
                case OptionType.PaintBrush: case OptionType.PaintWatercolorPen: case OptionType.PaintPencil: case OptionType.PaintEraseBrush: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }

        InkRender InkRender;
        readonly InverseProportion InkRenderIP = new InverseProportion
        {
            Minimum = 1,
            Maximum = 20
        };
        bool IsEnable = true;

        public InkPresenter InkPresenter { get; set; }
        public CanvasDevice CanvasDevice { get; set; }

        readonly SizePickerExtension Step = new SizePickerExtension();

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="PaintMenu"/>. </summary>
        public OptionType Type
        {
            get => (OptionType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "ApplicationTitleBarExtension.IsAccent" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(OptionType), typeof(ApplicationTitleBarExtension), new PropertyMetadata(default(OptionType)));


        public string MaskTexture { get; private set; }
        public void SetMaskTexture(string texture)
        {
            this.MaskTexture = texture;
            this.MaskImage.UriSource = new System.Uri(texture);
        }
        public void CloseMask()
        {
            this.IsEnable = false;
            this.MaskButton.IsOn = false;
            this.IsEnable = true;
        }


        public string PatternTexture { get; private set; }
        public void SetPatternTexture(string texture)
        {
            this.PatternTexture = texture;
            this.PatternImage.UriSource = new System.Uri(texture);
        }
        public void SetStep(int step)
        {
            this.Step.Size = step;
            this.StepTextBox.Text = this.Step.ToString();
        }
        public void ClosePattern()
        {
            this.IsEnable = false;
            this.MaskButton.IsOn = false;
            this.IsEnable = true;
        }


        #endregion


        //@Construct
        public PaintMenu()
        {
            this.InitializeComponent();

            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.InkRender = new InkRender(sender, 320, 68);
                this.Update();
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.InkRender is null) return;
                args.DrawingSession.DrawImage(this.InkRender.Source);
            };

            this.SizeSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;

                if (this.InkRender is null) return;
                this.Update();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;

                this.CanvasControl.Opacity = opacity;
            };
            this.SpacingSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double spacing = this.SpacingRange.ConvertXToY(e.NewValue);
                double spacing2 = System.Math.Clamp(spacing / 100, 0.1, 4);
                this.InkPresenter.Spacing = (float)spacing2;

                if (this.InkRender is null) return;
                this.Update();
                this.CanvasControl.Invalidate(); // Invalidate
            };
            this.HardnessListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ElementIcon item)
                {
                    switch (item.Type)
                    {
                        case ElementType.BrushEdgeHardnessNone: this.InkPresenter.Hardness = BrushEdgeHardness.None; break;
                        case ElementType.BrushEdgeHardnessCosine: this.InkPresenter.Hardness = BrushEdgeHardness.Cosine; break;
                        case ElementType.BrushEdgeHardnessQuadratic: this.InkPresenter.Hardness = BrushEdgeHardness.Quadratic; break;
                        case ElementType.BrushEdgeHardnessCube: this.InkPresenter.Hardness = BrushEdgeHardness.Cube; break;
                        case ElementType.BrushEdgeHardnessQuartic: this.InkPresenter.Hardness = BrushEdgeHardness.Quartic; break;
                        default: this.InkPresenter.Hardness = BrushEdgeHardness.None; break;
                    }

                    if (this.InkRender is null) return;
                    this.Update();
                    this.CanvasControl.Invalidate(); // Invalidate
                }
            };

            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is BlendEffectMode item)
                {
                    this.InkPresenter.SetBlendMode(item.IsDefined(), item);
                }
            };

            this.MaskButton.Toggled += (s, e) =>
            {
                if (this.IsEnable is false) return;
                bool isOn = this.MaskButton.IsOn;
                if (isOn) this.MaskOpened?.Invoke(s, e); // Delegate
                else this.MaskClosed?.Invoke(s, e); // Delegate
            };
            this.PatternButton.Toggled += (s, e) =>
            {
                if (this.IsEnable is false) return;
                bool isOn = this.PatternButton.IsOn;
                if (isOn) this.PatternOpened?.Invoke(s, e); // Delegate
                else this.PatternClosed?.Invoke(s, e); // Delegate
            };

            this.RotateButton.Unchecked += (s, e) =>
            {
                if (this.IsEnable is false) return;
                this.InkPresenter.Rotate = false;
            };
            this.RotateButton.Checked += (s, e) =>
            {
                if (this.IsEnable is false) return;
                this.InkPresenter.Rotate = true;
            };

            this.StepTextBox.Text = this.Step.ToString();
            this.StepTextBox.KeyDown += (s, e) =>
            {
                if (this.IsEnable is false) return;
                if (SizePickerExtension.IsEnter(e.Key)) this.PatternButton.Focus(FocusState.Programmatic);
            };
            this.StepTextBox.LostFocus += (s, e) =>
            {
                if (this.IsEnable is false) return;
                if (this.Step.IsMatch(this.StepTextBox))
                {
                    this.InkPresenter.Step = this.Step.Size;
                }
            };
        }

        public void Update()
        {
            double size = this.InkRenderIP.ConvertOneToValue(this.SizeRange.YRange.IP.ConvertValueToOne(this.InkPresenter.Size));

            switch (this.Type)
            {
                case OptionType.PaintPencil:
                    switch (base.ActualTheme)
                    {
                        case ElementTheme.Light: this.InkRender.DrawLine((float)size, Colors.Black); break;
                        case ElementTheme.Dark: this.InkRender.DrawLine((float)size, Colors.White); break;
                    }
                    break;
                default:
                    switch (base.ActualTheme)
                    {
                        case ElementTheme.Light: this.InkRender.IsometricFillCircle((float)size, this.InkPresenter.Spacing, Colors.Black); break;
                        case ElementTheme.Dark: this.InkRender.IsometricFillCircle((float)size, this.InkPresenter.Spacing, Colors.White); break;
                    }
                    break;
            }
        }

        //@Strings
        public void ConstructStrings(ResourceLoader resource)
        {
        }

        public void Construct(PaintBrush brush)
        {
            this.IsEnable = false;
            {
                this.SizeSlider.Value = this.SizeRange.ConvertYToX(brush.Size);
                this.OpacitySlider.Value = brush.Opacity * 100;
                this.SpacingSlider.Value = brush.Spacing * 100;
                this.HardnessListView.SelectedIndex = (int)brush.Hardness;
                this.RotateButton.IsChecked = brush.Rotate;
                this.Step.Size = brush.Step;
                this.StepTextBox.Text = this.Step.ToString();
                this.CanvasControl.Invalidate(); // Invalidate
            }
            this.IsEnable = true;
        }


        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

    }
}