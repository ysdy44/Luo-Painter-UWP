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
                this.ComboBox.SelectedIndex = this.InkCollection.IndexOf(presenter.ToolType);


                // 1.Minimum
                this.SizeSlider.Minimum = this.SizeRange.XRange.Minimum;
                this.OpacitySlider.Minimum = 0d;
                this.SpacingSlider.Minimum = this.SpacingRange.XRange.Minimum;
                this.FlowSlider.Minimum = 0d;

                // 2.Value
                this.SizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
                this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);
                this.SpacingSlider.Value = this.SpacingRange.ConvertYToX(presenter.Spacing * 100);
                this.FlowSlider.Value = System.Math.Clamp(presenter.Flow * 100d, 0d, 100d);

                // 3.Maximum
                this.SizeSlider.Maximum = this.SizeRange.XRange.Maximum;
                this.OpacitySlider.Maximum = 100d;
                this.SpacingSlider.Maximum = this.SpacingRange.XRange.Maximum;
                this.FlowSlider.Maximum = 100d;


                this.IgnoreSizePressureButton.IsOn = presenter.IgnoreSizePressure;
                this.IgnoreFlowPressureButton.IsOn = presenter.IgnoreFlowPressure;

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

                this.BasisRadioButton.IsChecked = presenter.Mode is InkType.None;
                this.MixRadioButton.IsChecked = presenter.Mode is InkType.Mix;
                this.BlendRadioButton.IsChecked = presenter.Mode is InkType.Blend;

                this.BlendModeComboBox.SelectedIndex = this.BlendCollection.IndexOf(presenter.BlendMode);


                this.RotateButton.IsChecked = presenter.Rotate;
                this.StepTextBox.Text = presenter.Step.ToString();

                this.MaskButton.IsOn = presenter.AllowMask;
                this.MaskImage.UriSource = string.IsNullOrEmpty(presenter.MaskTexture) ? null : new System.Uri(presenter.MaskTexture);

                this.PatternButton.IsOn = presenter.AllowPattern;
                this.PatternImage.UriSource = string.IsNullOrEmpty(presenter.PatternTexture) ? null : new System.Uri(presenter.PatternTexture);
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
                case InkType.Brush_Mix:
                case InkType.Brush_Pattern_Mix:
                case InkType.Blur:
                case InkType.Erase:
                case InkType.Erase_Opacity:
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

                case InkType.Shape:
                case InkType.Shape_Pattern:
                case InkType.Shape_Opacity:
                case InkType.Shape_Pattern_Opacity:
                case InkType.Shape_Blend:
                case InkType.Shape_Pattern_Blend:
                case InkType.Shape_Opacity_Blend:
                case InkType.Shape_Pattern_Opacity_Blend:
                case InkType.Shape_Mix:
                case InkType.Shape_Pattern_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricShape(ds, this.Color, false);
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
                case InkType.Line_Mix:
                case InkType.Line_Pattern_Mix:
                case InkType.Mosaic:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.DrawLine(ds, this.Color);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Liquefy:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricShape(ds, this.Color, true);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                default:
                    break;
            }
        }

    }
}