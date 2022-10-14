using Luo_Painter.Blends;
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

        public void ConstructInk(InkPresenter presenter)
        {
            this.InkIsEnabled = false;
            {
                switch (presenter.ToolType)
                {
                    case InkType.Brush: this.ToolComboBox.SelectedIndex = 0; break;
                    case InkType.Circle: this.ToolComboBox.SelectedIndex = 1; break;
                    case InkType.Line: this.ToolComboBox.SelectedIndex = 2; break;
                    case InkType.Erase: this.ToolComboBox.SelectedIndex = 3; break;
                    case InkType.Liquefy: this.ToolComboBox.SelectedIndex = 4; break;
                    default: break;
                }

                // 1.Minimum
                this.SizeSlider.Minimum = this.SizeRange.XRange.Minimum;
                this.OpacitySlider.Minimum = 0d;
                this.SpacingSlider.Minimum = this.SpacingRange.XRange.Minimum;
                this.FlowSlider.Minimum = 0d;

                // 2.Value
                this.SizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
                this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);
                this.SpacingSlider.Value = this.SpacingRange.ConvertYToX(presenter.Spacing / 100);
                this.FlowSlider.Value = System.Math.Clamp(presenter.Flow * 100d, 0d, 100d);

                // 3.Maximum
                this.SizeSlider.Maximum = this.SizeRange.XRange.Maximum;
                this.OpacitySlider.Maximum = 100d;
                this.SpacingSlider.Maximum = this.SpacingRange.XRange.Maximum;
                this.FlowSlider.Maximum = 100d;


                switch (presenter.Shape)
                {
                    case Windows.UI.Input.Inking.PenTipShape.Circle:
                        this.ShapeListBox.SelectedIndex = presenter.IsStroke ? 1 : 0;
                        break;
                    case Windows.UI.Input.Inking.PenTipShape.Rectangle:
                        this.ShapeListBox.SelectedIndex = presenter.IsStroke ? 3 : 2;
                        break;
                    default:
                        break;
                }


                this.NoneRadioButton.IsChecked = presenter.Hardness is BrushEdgeHardness.None;
                this.CosineRadioButton.IsChecked = presenter.Hardness is BrushEdgeHardness.Cosine;
                this.QuadraticRadioButton.IsChecked = presenter.Hardness is BrushEdgeHardness.Quadratic;
                this.CubeRadioButton.IsChecked = presenter.Hardness is BrushEdgeHardness.Cube;
                this.QuarticRadioButton.IsChecked = presenter.Hardness is BrushEdgeHardness.Quartic;

                this.BlendRadioButton.IsChecked = presenter.Mode is InkType.None || presenter.Mode is InkType.Blend;
                this.BlurRadioButton.IsChecked = presenter.Mode is InkType.Blur;
                this.MixRadioButton.IsChecked = presenter.Mode is InkType.Mix;
                this.MosaicRadioButton.IsChecked = presenter.Mode is InkType.Mosaic;

                this.BlendModeComboBox.SelectedIndex = presenter.BlendMode.IsDefined() ? (int)presenter.BlendMode : 0;


                this.MaskButton.IsOn = presenter.AllowMask;
                this.MaskImage.UriSource = string.IsNullOrEmpty(presenter.MaskTexture) ? null : new System.Uri(presenter.MaskTexture);
                this.RotateButton.IsChecked = presenter.Rotate;


                this.PatternButton.IsOn = presenter.AllowPattern;
                this.PatternImage.UriSource = string.IsNullOrEmpty(presenter.PatternTexture) ? null : new System.Uri(presenter.PatternTexture);
                this.StepTextBox.Text = presenter.Step.ToString();
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