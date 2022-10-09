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
        private double PercentageConverter(double value) => System.Math.Clamp(value / 100d, 0d, 1d);

        private Visibility BooleanToVisibilityConverter(bool? value) => value is true ? Visibility.Visible : Visibility.Collapsed;
        private Visibility SpacingVisibilityConverter(InkType value) => value.HasFlag(InkType.UISpacing) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility BlendModeVisibilityConverter(InkType value) => value.HasFlag(InkType.UIBlendMode) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility HardnessVisibilityConverter(InkType value) => value.HasFlag(InkType.UIHardness) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility MaskVisibilityConverter(InkType value) => value.HasFlag(InkType.UIMask) ? Visibility.Visible : Visibility.Collapsed;
        private Visibility PatternVisibilityConverter(InkType value) => value.HasFlag(InkType.UIPattern) ? Visibility.Visible : Visibility.Collapsed;


        bool InkIsEnabled = true;


        #region DependencyProperty


        /// <summary> Gets or set the type for <see cref="BrushPage"/>. </summary>
        public InkType Type
        {
            get => (InkType)base.GetValue(TypeProperty);
            set => base.SetValue(TypeProperty, value);
        }
        /// <summary> Identifies the <see cref = "BrushPage.Type" /> dependency property. </summary>
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

        private void InkAsync()
        {
            //@Task
            if (System.Threading.Monitor.TryEnter(this.InkLocker, System.TimeSpan.FromMilliseconds(100)))
            {
                this.Ink();
                System.Threading.Monitor.Exit(this.InkLocker);
            }
            //else // Frame dropping
        }
        private void Ink()
        {
            switch (this.InkType)
            {
                case InkType.Brush:
                case InkType.Brush_Pattern:
                case InkType.Brush_Opacity:
                case InkType.Brush_Pattern_Opacity:
                case InkType.Brush_Blend:
                case InkType.Brush_Pattern_Blend:
                case InkType.Brush_Opacity_Blend:
                case InkType.Brush_Pattern_Opacity_Blend:
                case InkType.Brush_Blur:
                case InkType.Brush_Pattern_Blur:
                case InkType.Brush_Mosaic:
                case InkType.Brush_Pattern_Mosaic:
                case InkType.Brush_Mix:
                case InkType.Brush_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        //@DPI 
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricDrawShaderBrushEdgeHardness(ds, this.BrushEdgeHardnessShaderCodeBytes, this.ColorHdr, this.InkCanvasControl.Dpi.ConvertPixels());
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.MaskBrush:
                case InkType.MaskBrush_Pattern:
                case InkType.MaskBrush_Opacity:
                case InkType.MaskBrush_Pattern_Opacity:
                case InkType.MaskBrush_Blend:
                case InkType.MaskBrush_Pattern_Blend:
                case InkType.MaskBrush_Opacity_Blend:
                case InkType.MaskBrush_Pattern_Opacity_Blend:
                case InkType.MaskBrush_Blur:
                case InkType.MaskBrush_Pattern_Blur:
                case InkType.MaskBrush_Mosaic:
                case InkType.MaskBrush_Pattern_Mosaic:
                case InkType.MaskBrush_Mix:
                case InkType.MaskBrush_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        //@DPI
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkCanvasControl.Dpi.ConvertPixels());
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Circle:
                case InkType.Circle_Pattern:
                case InkType.Circle_Opacity:
                case InkType.Circle_Pattern_Opacity:
                case InkType.Circle_Blend:
                case InkType.Circle_Pattern_Blend:
                case InkType.Circle_Opacity_Blend:
                case InkType.Circle_Pattern_Opacity_Blend:
                case InkType.Circle_Blur:
                case InkType.Circle_Pattern_Blur:
                case InkType.Circle_Mosaic:
                case InkType.Circle_Pattern_Mosaic:
                case InkType.Circle_Mix:
                case InkType.Circle_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricFillCircle(ds, this.Color, false);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Line:
                case InkType.Line_Pattern:
                case InkType.Line_Opacity:
                case InkType.Line_Pattern_Opacity:
                case InkType.Line_Blend:
                case InkType.Line_Pattern_Blend:
                case InkType.Line_Opacity_Blend:
                case InkType.Line_Pattern_Opacity_Blend:
                case InkType.Line_Blur:
                case InkType.Line_Pattern_Blur:
                case InkType.Line_Mosaic:
                case InkType.Line_Pattern_Mosaic:
                case InkType.Line_Mix:
                case InkType.Line_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.DrawLine(ds, this.Color);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Erase:
                case InkType.Erase_Opacity:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricFillCircle(ds, this.Color, false);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Liquefy:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricFillCircle(ds, this.Color, true);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                default:
                    break;
            }
        }

    }
}