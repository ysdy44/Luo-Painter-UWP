using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        readonly DispatcherTimer TipTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        private void ConstructTip()
        {
            this.HideTipStoryboard.Completed += (s, e) =>
            {
                this.TipBorder.Visibility = Visibility.Collapsed;
            };
            this.TipTimer.Tick += (s, e) =>
            {
                this.TipTimer.Stop();
                this.HideTipStoryboard.Begin(); // Storyboard
            };
        }

        public void Tip(string title, string subtitle)
        {
            this.TipTitleTextBlock.Text = title;
            this.TipSubtitleTextBlock.Text = subtitle;
            this.TipBorder.Visibility = Visibility.Visible;

            this.HideTipStoryboard.Stop(); // Storyboard
            this.ShowTipStoryboard.Begin(); // Storyboard

            this.TipTimer.Stop();
            this.TipTimer.Start();
        }


        private void ConstructColor()
        {
            this.ColorComboBox.ItemsSource = Enum.GetValues(typeof(ColorSpectrumComponents));
            this.ColorComboBox.SelectedItem = this.ColorPicker.ColorSpectrumComponents;
            this.ColorComboBox.SelectionChanged += (s, e) =>
            {
                if (this.ColorComboBox.SelectedItem is ColorSpectrumComponents components)
                {
                    this.ColorPicker.ColorSpectrumComponents = components;
                }
            };
        }

        private void ConstructColorShape()
        {
            this.ColorShapeComboBox.ItemsSource = Enum.GetValues(typeof(ColorSpectrumShape));
            this.ColorShapeComboBox.SelectedItem = this.ColorPicker.ColorSpectrumShape;
            this.ColorShapeComboBox.SelectionChanged += (s, e) =>
            {
                if (this.ColorShapeComboBox.SelectedItem is ColorSpectrumShape shape)
                {
                    this.ColorPicker.ColorSpectrumShape = shape;
                }
            };
        }

    }
}