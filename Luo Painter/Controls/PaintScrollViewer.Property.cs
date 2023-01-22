using Luo_Painter.Brushes;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {

        public void ConstructProperty()
        {
            this.TypeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.InkIsEnabled is false) return;
                if (this.TypeComboBox.SelectedItem is InkType type)
                {
                    if (this.InkPresenter.Type == type) return;
                    this.InkPresenter.Type = type;

                    this.Type = type;
                    this.InkType = this.InkPresenter.GetType();
                    this.TryInkAsync();

                    this.ComboBox.SelectedIndex = 0;
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
        }

    }
}