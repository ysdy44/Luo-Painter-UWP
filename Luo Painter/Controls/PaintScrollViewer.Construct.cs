using Luo_Painter.Brushes;
using Luo_Painter.Layers;
using Microsoft.Graphics.Canvas.Effects;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        readonly BrushEdgeHardness[] Hardnesss = new BrushEdgeHardness[]
        {
            BrushEdgeHardness.None,
            BrushEdgeHardness.Cosine,
            BrushEdgeHardness.Quadratic,
            BrushEdgeHardness.Cube,
            BrushEdgeHardness.Quartic,
        };
        readonly BlendEffectMode[] BlendModes = new BlendEffectMode[]
        {
            // None
            BlendExtensions.None,
            // Darken 
            BlendEffectMode.Darken,
            BlendEffectMode.Multiply,
            // Lighten 
            BlendEffectMode.Lighten,
            BlendEffectMode.Screen,
            BlendEffectMode.LinearDodge,
            // Contrast 
            BlendEffectMode.Overlay,
            // Difference 
            BlendEffectMode.Difference,
            BlendEffectMode.Subtract,
        };

        public void ConstructInkSliderValue(InkPresenter presenter)
        {
            this.IsInkEnabled = false;
            {
                // 2.Value
                this.SizeSlider.Value = presenter.Size;
                this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);
            }
            this.IsInkEnabled = true;
        }

        public void ConstructInkSlider(InkPresenter presenter)
        {
            this.IsInkEnabled = false;
            {
                // 1.Minimum
                this.SizeSlider.Minimum = 1d;
                //this.OpacitySlider.Minimum = 0d;

                // 2.Value
                this.SizeSlider.Value = presenter.Size;
                this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);

                // 3.Maximum
                this.SizeSlider.Maximum = 400d;
                //this.OpacitySlider.Maximum = 100d;
            }
            this.IsInkEnabled = true;
        }

        public void ConstructInk(InkPresenter presenter)
        {
            this.IsInkEnabled = false;
            {
                this.Type = presenter.Type;

                for (int i = 0; i < this.Inks.Length; i++)
                {
                    if (this.Inks[i] == presenter.Type)
                    {
                        this.TypeComboBox.SelectedIndex = i;
                        break;
                    }
                }

                // Property
                // 1.Minimum
                this.SizeSlider.Minimum = 1d;
                //this.OpacitySlider.Minimum = 0d;
                this.SpacingSlider.Minimum = 10d;
                //this.FlowSlider.Minimum = 0d;

                // 2.Value
                this.SizeSlider.Value = presenter.Size;
                this.OpacitySlider.Value = System.Math.Clamp(presenter.Opacity * 100d, 0d, 100d);
                this.SpacingSlider.Value = presenter.Spacing * 100;
                this.FlowSlider.Value = System.Math.Clamp(presenter.Flow * 100d, 0d, 100d);

                // 3.Maximum
                this.SizeSlider.Maximum = 400d;
                //this.OpacitySlider.Maximum = 100d;
                this.SpacingSlider.Maximum = 400d;
                //this.FlowSlider.Maximum = 100d;


                this.MinSizeSlider.Value = System.Math.Clamp(presenter.MinSize * 100d, 0d, 100d);
                this.MinFlowSlider.Value = System.Math.Clamp(presenter.MinFlow * 100d, 0d, 100d);

                this.MinSizeSlider.IsEnabled = presenter.SizePressure != default;
                this.MinFlowSlider.IsEnabled = presenter.FlowPressure != default;

                this.SizePressurePolygon.Points = this.SizePressurePoints[presenter.SizePressure];
                this.FlowPressurePolygon.Points = this.FlowPressurePoints[presenter.FlowPressure];


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

                for (int i = 0; i < this.Hardnesss.Length; i++)
                {
                    if (this.Hardnesss[i] == presenter.Hardness)
                    {
                        this.HardnessListView.SelectedIndex = i;
                        break;
                    }
                }


                // Texture
                this.RotateButton.IsOn = presenter.Rotate;
                this.ShapeImage.UriSource = string.IsNullOrEmpty(presenter.Shape) ? null : new System.Uri(presenter.Shape.GetTexture());
                this.RecolorShapeButton.IsOn = this.ShapeImage.ShowAsMonochrome = presenter.RecolorShape;

                this.GrainImage.UriSource = string.IsNullOrEmpty(presenter.Grain) ? null : new System.Uri(presenter.Grain.GetTexture());
                this.RecolorGrainButton.IsOn = this.GrainImage.ShowAsMonochrome = presenter.RecolorGrain;

                for (int i = 0; i < this.BlendModes.Length; i++)
                {
                    if (this.BlendModes[i] == presenter.BlendMode)
                    {
                        this.BlendModeListView.SelectedIndex = i;
                        break;
                    }
                }


                // Texture
                // 1.Minimum
                this.GrainScaleSlider.Minimum = 25d;

                // 2.Value
                this.GrainScaleSlider.Value = presenter.GrainScale * 100;

                // 3.Maximum
                this.GrainScaleSlider.Maximum = 425d;


                // Mix
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
            }
            this.IsInkEnabled = true;
        }

    }
}