using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class BrushPage : Page
    {

        //@Converter
        private string RoundConverter(double value) => $"{value:0}";
        private string SizeXToYConverter(double value) => this.RoundConverter(this.SizeRange.ConvertXToY(value));
        private string SpacingXToYConverter(double value) => this.RoundConverter(this.SpacingRange.ConvertXToY(value));
        private bool BooleanConverter(bool? value) => value is true;

        private Visibility BooleanToVisibilityConverter(bool? value) => value is true ? Visibility.Visible : Visibility.Collapsed;
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
                case InkType.Brush: case InkType.Circle: return Visibility.Visible;
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

        bool InkIsEnabled = true;


        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="PaintMenu"/>. </summary>
        public InkType Type
        {
            get => (InkType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "PaintMenu.IsAccent" /> dependency property. </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(InkType), typeof(BrushPage), new PropertyMetadata(default(InkType)));


        #endregion

        public void ConstructInk(PaintBrush brush)
        {
            this.InkIsEnabled = false;
            {
                this.SizeSlider.Value = this.SizeRange.ConvertYToX(brush.Size);
                this.OpacitySlider.Value = brush.Opacity * 100;
                this.SpacingSlider.Value = this.SpacingRange.ConvertYToX(brush.Spacing * 100);

                this.NoneRadioButton.IsChecked = brush.Hardness is BrushEdgeHardness.None;
                this.CosineRadioButton.IsChecked = brush.Hardness is BrushEdgeHardness.Cosine;
                this.QuadraticRadioButton.IsChecked = brush.Hardness is BrushEdgeHardness.Quadratic;
                this.CubeRadioButton.IsChecked = brush.Hardness is BrushEdgeHardness.Cube;
                this.QuarticRadioButton.IsChecked = brush.Hardness is BrushEdgeHardness.Quartic;

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
            this.InkIsEnabled = true;
        }

        private void Ink(CanvasDrawingSession ds)
        {
            double size = this.InkPresenter.Size / 24 + 1;
            switch (this.InkType)
            {
                case InkType.Brush_Dry:
                case InkType.Brush_Wet_Pattern:
                case InkType.Brush_Wet_Opacity:
                case InkType.Brush_Wet_Pattern_Opacity:
                case InkType.Brush_WetComposite_Blend:
                case InkType.Brush_WetComposite_Pattern_Blend:
                case InkType.Brush_WetComposite_Opacity_Blend:
                case InkType.Brush_WetComposite_Pattern_Opacity_Blend:
                case InkType.Brush_Dry_Mix:
                case InkType.Brush_Wet_Pattern_Mix:
                case InkType.Brush_WetBlur_Blur:
                case InkType.Brush_WetBlur_Pattern_Blur:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, Vector4.One, this.InkPresenter.Size, this.InkPresenter.Spacing, (int)this.InkPresenter.Hardness);
                    break;
                case InkType.MaskBrush_Dry:
                case InkType.MaskBrush_Wet_Pattern:
                case InkType.MaskBrush_Wet_Opacity:
                case InkType.MaskBrush_Wet_Pattern_Opacity:
                case InkType.MaskBrush_WetComposite_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Blend:
                case InkType.MaskBrush_WetComposite_Opacity_Blend:
                case InkType.MaskBrush_WetComposite_Pattern_Opacity_Blend:
                case InkType.MaskBrush_Dry_Mix:
                case InkType.MaskBrush_Wet_Pattern_Mix:
                case InkType.MaskBrush_WetBlur_Blur:
                case InkType.MaskBrush_WetBlur_Pattern_Blur:
                    this.InkRender.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, Vector4.One, this.InkPresenter.Mask, this.InkPresenter.Rotate, this.InkPresenter.Size, this.InkPresenter.Spacing, (int)this.InkPresenter.Hardness);
                    break;
                case InkType.Line_Dry:
                case InkType.Line_Wet_Pattern:
                case InkType.Line_Wet_Opacity:
                case InkType.Line_Wet_Pattern_Opacity:
                case InkType.Line_WetComposite_Blend:
                case InkType.Line_WetComposite_Pattern_Blend:
                case InkType.Line_WetComposite_Opacity_Blend:
                case InkType.Line_WetComposite_Pattern_Opacity_Blend:
                case InkType.Line_Dry_Mix:
                case InkType.Line_Wet_Pattern_Mix:
                case InkType.Line_WetBlur_Blur:
                case InkType.Line_WetBlur_Pattern_Blur:
                    this.InkRender.DrawLine(ds, (float)size, Colors.White);
                    break;
                case InkType.Liquefy:
                    this.InkRender.IsometricFillCircle(ds, Colors.White, (float)size, 0.25f);
                    break;
                default:
                    this.InkRender.IsometricFillCircle(ds, Colors.White, (float)size, this.InkPresenter.Spacing);
                    break;
            }
        }

    }
}