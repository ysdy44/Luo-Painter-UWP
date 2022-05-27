using FanKit.Transformers;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class StretchDialog : ContentDialog
    {

        //@Converter
        private string Round2Converter(double value) => $"{System.Math.Round(value, 2)}";
        private Visibility BooleanToVisibilityConverter(bool value) => value ? Visibility.Visible : Visibility.Collapsed;
        private Visibility ReverseBooleanToVisibilityConverter(bool value) => value ? Visibility.Collapsed : Visibility.Visible;

        //@Content
        public BitmapSize Size => this.SizePicker.Size;
        public IndicatorMode IndicatorMode => this.IndicatorControl.Mode;
        public CanvasImageInterpolation Interpolation
        {
            get
            {
                switch (this.InterpolationListView.SelectedIndex)
                {
                    case 0: return CanvasImageInterpolation.NearestNeighbor;
                    case 1: return CanvasImageInterpolation.Linear;
                    case 2: return CanvasImageInterpolation.Cubic;
                    case 3: return CanvasImageInterpolation.MultiSampleLinear;
                    case 4: return CanvasImageInterpolation.Anisotropic;
                    default: return CanvasImageInterpolation.HighQualityCubic;
                }
            }
        }

        //@Construct
        public StretchDialog()
        {
            this.InitializeComponent();
            this.InterpolationListView.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Windows.Foundation.Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.Border.Width = e.NewSize.Width;
                this.Border.Height = e.NewSize.Height;
            };

            this.ModeRun.Text = this.IndicatorControl.Mode.ToString();
            this.IndicatorControl.ModeChanged += (sender, mode) =>
            {
                this.ModeRun.Text = mode.ToString();
            };
        }

        public void Resezing(int width, int height)
        {
            this.SizePicker.ResezingWidth(width);
            this.SizePicker.ResezingHeight(height);
        }

    }
}