using Luo_Painter.Brushes;
using Windows.UI.Input.Inking;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        public void ConstructProperty()
        {
            this.TypeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;

                InkType type = this.Inks[this.TypeComboBox.SelectedIndex];

                if (this.InkPresenter.Type == type) return;
                this.InkPresenter.Type = type;

                this.Type = type;
                this.InkType = this.InkPresenter.GetType();
                this.TryInkAsync();

                this.ComboBox.SelectedIndex = 0;
            };


            this.SizeSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double size = System.Math.Max(e.NewValue, 0);
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
                double spacing = System.Math.Clamp(e.NewValue / 100, 0.1, 4);
                this.InkPresenter.Spacing = (float)spacing;
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

            this.MinSizeSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double flow = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.MinSize = (float)flow;
                this.InkType = this.InkPresenter.GetType();
                this.TryInkAsync();
            };
            this.MinFlowSlider.ValueChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                double flow = System.Math.Clamp(e.NewValue / 100, 0, 1);
                this.InkPresenter.MinFlow = (float)flow;
                this.InkType = this.InkPresenter.GetType();
                this.TryInkAsync();
            };

            this.SizePressureButton.Click += (s, e) =>
            {
                this.PressurePicker.Construct(this.InkPresenter.SizePressure);
                this.PressureFlyout.ShowAt(this.SizePressureButton);
            };
            this.FlowPressureButton.Click += (s, e) =>
            {
                this.PressurePicker.Construct(this.InkPresenter.FlowPressure);
                this.PressureFlyout.ShowAt(this.FlowPressureButton);
            };
            this.PressurePicker.PressureChanged += (s, pressure) =>
            {
                if (this.PressureFlyout.Target == this.SizePressureButton)
                {
                    this.MinSizeSlider.IsEnabled = pressure != default;
                    this.SizePressurePolygon.Points = this.SizePressurePoints[pressure];

                    this.InkPresenter.SizePressure = pressure;
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInk();
                }
                else if (this.PressureFlyout.Target == this.FlowPressureButton)
                {
                    this.MinFlowSlider.IsEnabled = pressure != default;
                    this.FlowPressurePolygon.Points = this.FlowPressurePoints[pressure];

                    this.InkPresenter.FlowPressure = pressure;
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInk();
                }
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


            this.HardnessListView.ItemClick += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (e.ClickedItem is BrushEdgeHardness item)
                {
                    this.InkPresenter.Hardness = item;
                    this.TryInk();
                }
            };
        }

    }
}