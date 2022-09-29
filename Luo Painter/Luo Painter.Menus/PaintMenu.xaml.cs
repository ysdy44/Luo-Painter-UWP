using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Luo_Painter.Menus
{
    public sealed partial class PaintMenu : Expander
    {
        //@Delegate  
        public event EventHandler<InkType> InkModeChanged;

        public event EventHandler<float> InkSizeChanged;
        public event EventHandler<float> InkOpacityChanged;
        public event EventHandler<float> InkSpacingChanged;
        public event EventHandler<BrushEdgeHardness> InkHardnessChanged;

        public event EventHandler<bool> InkRotateChanged;
        public event EventHandler<int> InkStepChanged;

        public event EventHandler<BlendEffectMode> InkBlendModeChanged;

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
        private bool Int0ToBooleanConverter(int value) => value is 0;
        private Visibility Int056ToVisibility(InkType value)
        {
            switch (value)
            {
                case InkType.Brush: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }
        private Visibility Int012ToVisibility(InkType value)
        {
            switch (value)
            {
                case InkType.Brush: case InkType.Circle: case InkType.Line: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }
        private Visibility Int0125ToVisibility(InkType value)
        {
            switch (value)
            {
                case InkType.Brush: case InkType.Circle: case InkType.Line: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }
        private Visibility Int013ToVisibility(InkType value)
        {
            switch (value)
            {
                case InkType.Brush: case InkType.Circle: case InkType.Erase: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }
        private Visibility Int0123ToVisibility(InkType value)
        {
            switch (value)
            {
                case InkType.Brush: case InkType.Circle: case InkType.Line: case InkType.Erase: return Visibility.Visible;
                default: return Visibility.Collapsed;
            }
        }

        bool IsEnable;

        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="PaintMenu"/>. </summary>
        public InkType Type
        {
            get => (InkType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "PaintMenu.IsAccent" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(InkType), typeof(PaintMenu), new PropertyMetadata(default(InkType)));


        public string MaskTexture => this.MaskImage.UriSource?.ToString();
        public void SetMaskTexture(string texture) => this.MaskImage.UriSource = new System.Uri(texture);
        public void CloseMask()
        {
            if (this.MaskButton.IsOn is false) return;

            this.IsEnable = false;
            this.MaskButton.IsOn = false;
            this.IsEnable = true;
        }


        public string PatternTexture => this.PatternImage.UriSource?.ToString();
        public void SetPatternTexture(string texture) => this.PatternImage.UriSource = new System.Uri(texture);
        public void SetStep(int step)
        {
            this.Step.Size = step;
            this.StepTextBox.Text = this.Step.ToString();
        }
        public void ClosePattern()
        {
            if (this.PatternButton.IsOn is false) return;

            this.IsEnable = false;
            this.PatternButton.IsOn = false;
            this.IsEnable = true;
        }


        #endregion


        //@Construct
        public PaintMenu()
        {
            this.InitializeComponent();
            base.Unloaded += (s, e) => this.IsEnable = false;
            base.Loaded += (s, e) => this.IsEnable = true;
            this.SizeSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkSizeChanged?.Invoke(this, (float)size); // Delegate
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkOpacityChanged?.Invoke(this, (float)opacity); // Delegate
            };
            this.SpacingSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double spacing = this.SpacingRange.ConvertXToY(e.NewValue);
                double spacing2 = System.Math.Clamp(spacing / 100, 0.1, 4);
                this.InkSpacingChanged?.Invoke(this, (float)spacing2); // Delegate
            };
            this.HardnessListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ElementIcon item)
                {
                    switch (item.Type)
                    {
                        case ElementType.BrushEdgeHardnessNone:
                            this.InkHardnessChanged?.Invoke(this, BrushEdgeHardness.None); // Delegate
                            break;
                        case ElementType.BrushEdgeHardnessCosine:
                            this.InkHardnessChanged?.Invoke(this, BrushEdgeHardness.Cosine); // Delegate
                            break;
                        case ElementType.BrushEdgeHardnessQuadratic:
                            this.InkHardnessChanged?.Invoke(this, BrushEdgeHardness.Quadratic); // Delegate
                            break;
                        case ElementType.BrushEdgeHardnessCube:
                            this.InkHardnessChanged?.Invoke(this, BrushEdgeHardness.Cube); // Delegate
                            break;
                        case ElementType.BrushEdgeHardnessQuartic:
                            this.InkHardnessChanged?.Invoke(this, BrushEdgeHardness.Quartic); // Delegate
                            break;
                        default:
                            break;
                    }
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
                this.InkRotateChanged?.Invoke(s, false); // Delegate
            };
            this.RotateButton.Checked += (s, e) =>
            {
                if (this.IsEnable is false) return;
                this.InkRotateChanged?.Invoke(s, true); // Delegate
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
                    this.InkStepChanged?.Invoke(s, this.Step.Size); // Delegate
                }
            };


            this.ModeComboBox.SelectionChanged += (s, e) =>
            {
                int item = this.ModeComboBox.SelectedIndex;
                switch (item)
                {
                    case 1:
                        this.InkModeChanged?.Invoke(this, InkType.Blur); // Delegate
                        break;
                    case 2:
                        this.InkModeChanged?.Invoke(this, InkType.Mosaic); // Delegate
                        break;
                    case 3:
                        this.InkModeChanged?.Invoke(this, InkType.Mix); // Delegate
                        break;
                    default:
                        switch (this.BlendModeComboBox.SelectedIndex)
                        {
                            case 0:
                                this.InkModeChanged?.Invoke(this, InkType.None); // Delegate
                                break;
                            default:
                                this.InkModeChanged?.Invoke(this, InkType.Blend); // Delegate
                                break;
                        }
                        break;
                }
            };


            this.BlendModeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;

                if (this.BlendModeComboBox.SelectedItem is int item)
                {
                    this.InkBlendModeChanged?.Invoke(this, (BlendEffectMode)item); // Delegate
                }
            };
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
                this.SpacingSlider.Value = this.SpacingRange.ConvertYToX(brush.Spacing * 100);
                this.HardnessListView.SelectedIndex = (int)brush.Hardness;

                if (brush.Mask is PaintTexture mask)
                {
                    this.MaskButton.IsOn = true;
                    this.RotateButton.IsChecked = brush.Rotate;
                    this.MaskImage.UriSource = new System.Uri(mask.Texture);
                }
                else
                {
                    this.MaskButton.IsOn = false;
                    this.RotateButton.IsChecked = false;
                }

                if (brush.Pattern is PaintTexture pattern)
                {
                    this.PatternButton.IsOn = true;
                    this.PatternImage.UriSource = new System.Uri(pattern.Texture);
                    this.Step.Size = pattern.Step;
                    this.StepTextBox.Text = this.Step.ToString();
                }
                else
                {
                    this.PatternButton.IsOn = false;
                    this.Step.Size = 1024;
                    this.StepTextBox.Text = 1024.ToString();
                }
            }
            this.IsEnable = true;
        }

    }
}