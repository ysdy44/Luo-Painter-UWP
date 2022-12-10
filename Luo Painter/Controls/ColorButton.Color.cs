using Windows.UI;

namespace Luo_Painter.Controls
{
    public sealed partial class ColorButton
    {

        private void ConstructColor()
        {
            this.TricolorPicker.ColorChanged += (s, color) => this.Color2(color);
            this.HuePicker.ColorChanged += (s, color) => this.Color2(color);

            this.TricolorPicker.ColorChangedCompleted += (s, color) => this.Swatches(color);
            this.HuePicker.ColorChangedCompleted += (s, color) => this.Swatches(color);

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is Color item)
                {
                    this.Color2(item);
                }
            };
        }

        private void Color2(Color color)
        {
            this.PrimarySolidColorBrush.Color = color;
            this.SolidColorBrush.Color = color;

            this.SetColor(color);
            this.SetColorHdr(color);
            this.ColorChanged?.Invoke(this, color); // Delegate
        }
        private void Color3(Color color)
        {
            this.PrimarySolidColorBrush.Color = color;
            this.SecondarySolidColorBrush.Color = color;
            this.SolidColorBrush.Color = color;

            this.SetColor(color);
            this.SetColorHdr(color);
            this.ColorChanged?.Invoke(this, color); // Delegate
        }

        private void Swatches(Color color)
        {
            foreach (Color item in this.ObservableCollection)
            {
                if (item == color) return;
            }

            while (this.ObservableCollection.Count > 10)
            {
                this.ObservableCollection.RemoveAt(0);
            }
            this.ObservableCollection.Add(color);
        }

    }
}