using Luo_Painter.Brushes;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    internal enum PaintNumberPickerMode : byte
    {
        None,

        Size,
        Opacity,
        Spacing,
        Flow,

        Mix,
        Wet,
        Persistence,
    }

    public sealed partial class PaintScrollViewer : UserControl, IInkParameter
    {

        PaintNumberPickerMode NumberPickerMode;

        private void NumberShowAt(INumberSlider slider, PaintNumberPickerMode mode = default)
        {
            this.NumberPickerMode = mode;
            this.NumberFlyout.ShowAt(slider.PlacementTarget);
            this.NumberPicker.Construct(slider);
        }

        public void ConstructPicker()
        {
            this.SizeSlider.Click += (s, e) => this.NumberShowAt(this.SizeSlider, PaintNumberPickerMode.Size);
            this.OpacitySlider.Click += (s, e) => this.NumberShowAt(this.OpacitySlider, PaintNumberPickerMode.Opacity);
            this.SpacingSlider.Click += (s, e) => this.NumberShowAt(this.SpacingSlider, PaintNumberPickerMode.Spacing);
            this.FlowSlider.Click += (s, e) => this.NumberShowAt(this.FlowSlider, PaintNumberPickerMode.Flow);

            this.MixSlider.Click += (s, e) => this.NumberShowAt(this.MixSlider, PaintNumberPickerMode.Mix);
            this.WetSlider.Click += (s, e) => this.NumberShowAt(this.WetSlider, PaintNumberPickerMode.Wet);
            this.PersistenceSlider.Click += (s, e) => this.NumberShowAt(this.PersistenceSlider, PaintNumberPickerMode.Persistence);

            this.NumberPicker.SecondaryButtonClick += (s, e) => this.NumberFlyout.Hide();
            this.NumberPicker.PrimaryButtonClick += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                switch (this.NumberPickerMode)
                {
                    case PaintNumberPickerMode.None:
                        break;

                    case PaintNumberPickerMode.Size:
                        this.SizeSlider.Value = this.SizeRange.ConvertYToX(e);
                        break;
                    case PaintNumberPickerMode.Opacity:
                        this.OpacitySlider.Value = e;
                        break;
                    case PaintNumberPickerMode.Spacing:
                        this.SpacingSlider.Value = this.SpacingRange.ConvertYToX(e);
                        break;
                    case PaintNumberPickerMode.Flow:
                        this.FlowSlider.Value = e;
                        break;

                    case PaintNumberPickerMode.Mix:
                        break;
                    case PaintNumberPickerMode.Wet:
                        break;
                    case PaintNumberPickerMode.Persistence:
                        break;

                    default:
                        break;
                }
            };
        }

    }
}