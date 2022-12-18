using Windows.UI;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton
    {

        private void ConstructColor()
        {
            this.TricolorPicker.ColorChanged += (s, color) => this.OnColorChanged(color, ColorChangedMode.WithPrimaryBrush | ColorChangedMode.WithColor);
            this.HuePicker.ColorChanged += (s, color) => this.OnColorChanged(color, ColorChangedMode.WithPrimaryBrush | ColorChangedMode.WithColor);

            this.TricolorPicker.ColorChangedCompleted += (s, color) => this.Swatches(color);
            this.HuePicker.ColorChangedCompleted += (s, color) => this.Swatches(color);

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Color item)
                {
                    this.OnColorChanged(item, ColorChangedMode.WithPrimaryBrush | ColorChangedMode.WithColor);
                }
            };
        }

        private void Swatches(Color color)
        {
            foreach (Color item in this.ObservableCollection)
            {
                if (item == color) return;
            }

            while (this.ObservableCollection.Count > 9)
            {
                this.ObservableCollection.RemoveAt(9);
            }
            this.ObservableCollection.Insert(0, color);
        }

    }
}