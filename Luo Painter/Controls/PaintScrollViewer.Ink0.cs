using Luo_Painter.Brushes;
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
                //this.OpacitySlider.Minimum = 0d;
                this.SpacingSlider.Minimum = this.SpacingRange.XRange.Minimum;
                //this.FlowSlider.Minimum = 0d;

                // 2.Value
                this.SizeSlider.Value = this.SizeRange.ConvertYToX(presenter.Size);
                this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);
                this.SpacingSlider.Value = this.SpacingRange.ConvertYToX(presenter.Spacing * 100);
                this.FlowSlider.Value = System.Math.Clamp(presenter.Flow * 100d, 0d, 100d);

                // 3.Maximum
                this.SizeSlider.Maximum = this.SizeRange.XRange.Maximum;
                //this.OpacitySlider.Maximum = 100d;
                this.SpacingSlider.Maximum = this.SpacingRange.XRange.Maximum;
                //this.FlowSlider.Maximum = 100d;


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


                this.BlendModeListView.SelectedIndex = this.BlendCollection.IndexOf(presenter.BlendMode);

                // 1.Minimum
                //this.MixSlider.Minimum = 0d;
                this.WetSlider.Minimum = 10d;
                //this.PersistenceSlider.Minimum = 0d;

                // 2.Value
                this.MixSlider.Value = System.Math.Clamp(presenter.Mix * 100d, 0d, 100d);
                this.WetSlider.Value = presenter.Wet;
                this.PersistenceSlider.Value = System.Math.Clamp(presenter.Persistence * 100d, 0d, 100d);

                // 3.Maximum
                //this.MixSlider.Maximum = 100d;
                this.WetSlider.Maximum = 20d;
                //this.PersistenceSlider.Maximum = 100d;


                this.RotateButton.IsChecked = presenter.Rotate;
                this.StepTextBox.Text = presenter.Step.ToString();

                this.ShapeButton.IsOn = presenter.AllowShape;
                this.ShapeImage.UriSource = string.IsNullOrEmpty(presenter.Shape) ? null : new System.Uri(presenter.Shape.GetTexture());

                this.GrainButton.IsOn = presenter.AllowGrain;
                this.GrainImage.UriSource = string.IsNullOrEmpty(presenter.Grain) ? null : new System.Uri(presenter.Grain.GetTexture());
            }
            this.InkIsEnabled = true;
        }

    }
}