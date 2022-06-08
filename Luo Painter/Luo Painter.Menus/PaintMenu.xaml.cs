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
    internal sealed class SizeRange
    {
        public readonly Range XRange;
        public readonly Range YRange = new Range
        {
            Default = 12,
            IP = new InverseProportion
            {
                Minimum = 1,
                Maximum = 400,
            }
        };

        private readonly InverseProportion XIP;
        private readonly InverseProportion YIP = new InverseProportion
        {
            Minimum = 0.3333333333333333333333333333333333333333333333333333333333333,
            Maximum = 1,
        };

        public SizeRange()
        {
            this.XIP = this.YIP.Convert();
            this.XRange = this.YRange.Convert(this.YIP, this.YRange.IP, 100000);
        }

        public double ConvertXToY(double x) => InverseProportion.Convert(x, this.XIP, this.XRange.IP, this.YIP, this.YRange.IP, RangeRounding.Maximum, RangeRounding.Minimum);
        public double ConvertYToX(double y) => InverseProportion.Convert(y, this.YIP, this.YRange.IP, this.XIP, this.XRange.IP, RangeRounding.Minimum, RangeRounding.Maximum);
    }

    internal sealed class InkRender
    {
        readonly BitmapLayer PaintLayer;
        public ICanvasImage Souce => this.PaintLayer.Source;
        public InkRender(CanvasControl sender) => this.PaintLayer = new BitmapLayer(sender, (int)sender.ActualWidth, (int)sender.ActualHeight);
        public void Render(InkPresenter presenter, Color color)
        {
            this.PaintLayer.Clear(Colors.Transparent, BitmapType.Origin);
            this.PaintLayer.Clear(Colors.Transparent, BitmapType.Source);

            float width = this.PaintLayer.Width;
            float height = this.PaintLayer.Height;
            float space = System.Math.Max(2, presenter.Size / height * 2);

            Vector2 position = new Vector2(10, height / 2 + 3.90181f);
            float pressure = 0.001f;

            for (float x = 10; x < width - 10; x += space)
            {
                // 0 ~ Π
                float radian = x / width * FanKit.Math.Pi;

                // Sin 0 ~ Π ︵
                float targetPressure = (float)System.Math.Sin(radian);
                // Sin 0 ~ 2Π ~
                float offsetY = 20 * (float)System.Math.Sin(radian + radian);
                Vector2 targetPosition = new Vector2(x, height / 2 + offsetY);

                this.PaintLayer.IsometricFillCircle(position, targetPosition, pressure, targetPressure, space, color, BitmapType.Source);
                position = targetPosition;
                pressure = targetPressure;
            }
        }
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
                this.InkRender = new InkRender(sender);
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light:
                        this.InkRender.Render(this.InkPresenter, Colors.Black);
                        break;
                    case ElementTheme.Dark:
                        this.InkRender.Render(this.InkPresenter, Colors.White);
                        break;
                }
            };
            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.InkRender is null) return;
                args.DrawingSession.DrawImage(this.InkRender.Souce);
            };

            this.SizeSlider.ValueChanged += (s, e) =>
            {
                if (this.IsEnable is false) return;
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;

                if (this.InkRender is null) return;
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light: this.InkRender.Render(this.InkPresenter, Colors.Black); break;
                    case ElementTheme.Dark: this.InkRender.Render(this.InkPresenter, Colors.White); break;
                }
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
                double spacing = System.Math.Clamp(e.NewValue / 100, 0.1, 4);
                this.InkPresenter.Spacing = (float)spacing;

                if (this.InkRender is null) return;
                switch (base.ActualTheme)
                {
                    case ElementTheme.Light: this.InkRender.Render(this.InkPresenter, Colors.Black); break;
                    case ElementTheme.Dark: this.InkRender.Render(this.InkPresenter, Colors.White); break;
                }
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
                    switch (base.ActualTheme)
                    {
                        case ElementTheme.Light: this.InkRender.Render(this.InkPresenter, Colors.Black); break;
                        case ElementTheme.Dark: this.InkRender.Render(this.InkPresenter, Colors.White); break;
                    }
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