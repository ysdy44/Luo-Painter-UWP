using Luo_Painter.Blends;
using Luo_Painter.Brushes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

                this.BlendRadioButton.IsChecked = presenter.Mode is InkType.None || presenter.Mode is InkType.Blend;
                this.BlurRadioButton.IsChecked = presenter.Mode is InkType.Blur;
                this.MixRadioButton.IsChecked = presenter.Mode is InkType.Mix;
                this.MosaicRadioButton.IsChecked = presenter.Mode is InkType.Mosaic;

                this.BlendModeComboBox.SelectedIndex = presenter.BlendMode.IsDefined() ? (int)presenter.BlendMode : 0;


                this.RotateButton.IsChecked = presenter.Rotate;
                this.StepTextBox.Text = presenter.Step.ToString();

                this.MaskButton.IsOn = presenter.AllowMask;
                this.MaskImage.UriSource = string.IsNullOrEmpty(presenter.MaskTexture) ? null : new System.Uri(presenter.MaskTexture);

                this.PatternButton.IsOn = presenter.AllowPattern;
                this.PatternImage.UriSource = string.IsNullOrEmpty(presenter.PatternTexture) ? null : new System.Uri(presenter.PatternTexture);
            }
            this.InkIsEnabled = true;
        }

        public void ConstructInk0()
        {
            this.ToolComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                switch (this.ToolComboBox.SelectedIndex)
                {
                    case 0: // OptionType.PaintBrush
                        {
                            if (this.InkPresenter.ToolType == InkType.Brush) return;
                            this.InkPresenter.ToolType = InkType.Brush;

                            this.Type = InkType.Brush;
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            System.Threading.Tasks.Task.Run(this.InkAsync);
                        }
                        break;
                    case 1: // OptionType.PaintWatercolorPen
                        {
                            if (this.InkPresenter.ToolType == InkType.Circle) return;
                            this.InkPresenter.ToolType = InkType.Circle;

                            this.Type = InkType.Circle;
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            System.Threading.Tasks.Task.Run(this.InkAsync);
                        }
                        break;
                    case 2: // OptionType.PaintPencil
                        {
                            if (this.InkPresenter.ToolType == InkType.Line) return;
                            this.InkPresenter.ToolType = InkType.Line;

                            this.Type = InkType.Line;
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            System.Threading.Tasks.Task.Run(this.InkAsync);
                        }
                        break;
                    case 3: // OptionType.PaintEraseBrush
                        {
                            if (this.InkPresenter.ToolType == InkType.Erase) return;
                            this.InkPresenter.ToolType = InkType.Erase;

                            this.Type = InkType.Erase;
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            System.Threading.Tasks.Task.Run(this.InkAsync);
                        }
                        break;
                    case 4: // OptionType.PaintLiquefaction
                        {
                            if (this.InkPresenter.ToolType == InkType.Liquefy) return;
                            this.InkPresenter.ToolType = InkType.Liquefy;
                            this.Type = InkType.Liquefy;
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            System.Threading.Tasks.Task.Run(this.InkAsync);
                        }
                        break;
                    default: // default
                        {
                            if (this.InkPresenter.ToolType == default) return;
                            this.InkPresenter.ToolType = default;

                            this.Type = default;
                            this.InkType = this.InkPresenter.GetType();

                            if (this.ShaderCodeByteIsEnabled is false) return;
                            System.Threading.Tasks.Task.Run(this.InkAsync);
                        }
                        break;
                }
            };
        }

    }
}