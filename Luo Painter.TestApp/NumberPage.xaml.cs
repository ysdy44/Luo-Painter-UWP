using Luo_Painter.HSVColorPickers;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.TestApp
{
    public sealed partial class NumberPage : Page
    {

        public NumberPage()
        {
            this.InitializeComponent();
            this.ConstructNumber();
            this.ConstructPicker();
        }

        public void ConstructNumber()
        {
            this.NumberButton.Click += (s, e) => this.NumberShowAt(this.NumberButton);
            this.NumberSlider.Click += (s, e) => this.NumberShowAt(this.NumberSlider);
        }

        private void NumberShowAt(INumberBase number)
        {
            this.NumberFlyout.ShowAt(number.PlacementTarget);
            this.NumberPicker.Construct(number);
        }

        public void ConstructPicker()
        {
            this.NumberFlyout.Closed += (s, e) => this.NumberPicker.Close();
            this.NumberFlyout.Opened += (s, e) => this.NumberPicker.Open();
            this.NumberPicker.ValueChanged += (s, e) =>
            {
                if (this.NumberFlyout.IsOpen is false) return;
                this.NumberFlyout.Hide();

                // Set Number
                this.NumberButton.Value = e;

                // Set Value
                this.NumberSlider.Value = e;

                // To String
                this.TextBlock.Text = this.NumberPicker.ToString();
            };
        }

    }
}