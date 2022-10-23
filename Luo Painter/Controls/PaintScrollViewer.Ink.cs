using Luo_Painter.Brushes;
using Luo_Painter.Elements;
using Microsoft.Graphics.Canvas;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        public void ConstructInk(InkPresenter presenter)
        {
            this.InkIsEnabled = false;
            {
                this.ComboBox.SelectedIndex = this.InkCollection.IndexOf(presenter.Type);


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

                switch (presenter.Tip)
                {
                    case Windows.UI.Input.Inking.PenTipShape.Circle:
                        this.TipListBox.SelectedIndex = presenter.IsStroke ? 1 : 0;
                        break;
                    case Windows.UI.Input.Inking.PenTipShape.Rectangle:
                        this.TipListBox.SelectedIndex = presenter.IsStroke ? 3 : 2;
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

                this.ShapeButton.IsOn = presenter.AllowShape;
                this.ShapeImage.UriSource = string.IsNullOrEmpty(presenter.ShapeTexture) ? null : new System.Uri(presenter.ShapeTexture);

                this.GrainButton.IsOn = presenter.AllowGrain;
                this.GrainImage.UriSource = string.IsNullOrEmpty(presenter.GrainTexture) ? null : new System.Uri(presenter.GrainTexture);
            }
            this.InkIsEnabled = true;
        }

        public void InkAsync()
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
                case InkType.General:
                case InkType.General_Grain:
                case InkType.General_Opacity:
                case InkType.General_Grain_Opacity:
                case InkType.General_Blend:
                case InkType.General_Grain_Blend:
                case InkType.General_Opacity_Blend:
                case InkType.General_Grain_Opacity_Blend:
                case InkType.General_Mix:
                case InkType.General_Grain_Mix:
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

                case InkType.ShapeGeneral:
                case InkType.ShapeGeneral_Grain:
                case InkType.ShapeGeneral_Opacity:
                case InkType.ShapeGeneral_Grain_Opacity:
                case InkType.ShapeGeneral_Blend:
                case InkType.ShapeGeneral_Grain_Blend:
                case InkType.ShapeGeneral_Opacity_Blend:
                case InkType.ShapeGeneral_Grain_Opacity_Blend:
                case InkType.ShapeGeneral_Mix:
                case InkType.ShapeGeneral_Grain_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        //@DPI
                        ds.Units = CanvasUnits.Pixels; /// <see cref="DPIExtensions">
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricDrawShaderBrushEdgeHardnessWithTexture(ds, this.BrushEdgeHardnessWithTextureShaderCodeBytes, this.ColorHdr, this.InkCanvasControl.Dpi.ConvertPixels());
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Tip:
                case InkType.Tip_Grain:
                case InkType.Tip_Opacity:
                case InkType.Tip_Grain_Opacity:
                case InkType.Tip_Blend:
                case InkType.Tip_Grain_Blend:
                case InkType.Tip_Opacity_Blend:
                case InkType.Tip_Grain_Opacity_Blend:
                case InkType.Tip_Mix:
                case InkType.Tip_Grain_Mix:
                    using (CanvasDrawingSession ds = this.InkRender.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Transparent);
                        this.InkPresenter.IsometricTip(ds, this.Color, false);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                case InkType.Line:
                case InkType.Line_Grain:
                case InkType.Line_Opacity:
                case InkType.Line_Grain_Opacity:
                case InkType.Line_Blend:
                case InkType.Line_Grain_Blend:
                case InkType.Line_Opacity_Blend:
                case InkType.Line_Grain_Opacity_Blend:
                case InkType.Line_Mix:
                case InkType.Line_Grain_Mix:
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
                        this.InkPresenter.IsometricTip(ds, this.Color, true);
                    }
                    this.InkCanvasControl.Invalidate();
                    break;

                default:
                    break;
            }
        }

    }
}