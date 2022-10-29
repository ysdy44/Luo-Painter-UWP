using Luo_Painter.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        public void ConstructInk1()
        {
            this.ComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (this.ComboBox.SelectedItem is InkType type)
                {
                    if (this.InkPresenter.Type == type) return;
                    this.InkPresenter.Type = type;

                    this.Type = type;
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInkAsync();
                }
            };


            this.SizeSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double size = this.SizeRange.ConvertXToY(e.NewValue);
                this.InkPresenter.Size = (float)size;
                this.TryInkAsync();
            };
            this.OpacitySlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double opacity = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Opacity = (float)opacity;
                this.InkType = this.InkPresenter.GetType();
            };
            this.SpacingSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double spacing = this.SpacingRange.ConvertXToY(e.NewValue);
                double spacing2 = System.Math.Clamp(spacing / 100, 0.1, 4);
                this.InkPresenter.Spacing = (float)spacing2;
                this.TryInkAsync();
            };
            this.FlowSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double flow = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Flow = (float)flow;
                this.InkType = this.InkPresenter.GetType();
                this.TryInkAsync();
            };


            this.IgnoreSizePressureButton.Toggled += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.IgnoreSizePressure = this.IgnoreSizePressureButton.IsOn;
                this.TryInkAsync();
            };
            this.IgnoreFlowPressureButton.Toggled += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.IgnoreFlowPressure = this.IgnoreFlowPressureButton.IsOn;
                this.TryInkAsync();
            };


            this.TipListBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                switch (this.TipListBox.SelectedIndex)
                {
                    case 0:
                        this.InkPresenter.Tip = PenTipShape.Circle;
                        this.InkPresenter.IsStroke = false;
                        this.TryInk();
                        break;
                    case 1:
                        this.InkPresenter.Tip = PenTipShape.Circle;
                        this.InkPresenter.IsStroke = true;
                        this.TryInk();
                        break;
                    case 2:
                        this.InkPresenter.Tip = PenTipShape.Rectangle;
                        this.InkPresenter.IsStroke = false;
                        this.TryInk();
                        break;
                    case 3:
                        this.InkPresenter.Tip = PenTipShape.Rectangle;
                        this.InkPresenter.IsStroke = true;
                        this.TryInk();
                        break;
                    default:
                        break;
                }
            };

            this.NoneRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.None;
                this.TryInk();
            };
            this.CosineRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Cosine;
                this.TryInk();
            };
            this.QuadraticRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Quadratic;
                this.TryInk();
            };
            this.CubeRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Cube;
                this.TryInk();
            };
            this.QuarticRadioButton.Checked += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                this.InkPresenter.Hardness = BrushEdgeHardness.Quartic;
                this.TryInk();
            };


            this.BlendModeListView.ItemClick += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (e.ClickedItem is BlendEffectMode item)
                {
                    this.InkPresenter.BlendMode = item;
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInk();
                }
            };

            this.MixSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double mix = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Mix = (float)mix;
                this.InkType = this.InkPresenter.GetType();
                this.TryInkAsync();
            };
            this.WetSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double wet = e.NewValue;
                this.InkPresenter.Wet = (float)wet;
                this.TryInkAsync();
            };
            this.PersistenceSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double persistence = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.Persistence = (float)persistence;
                this.TryInkAsync();
            };
        }

    }
}