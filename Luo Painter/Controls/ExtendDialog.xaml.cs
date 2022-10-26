using FanKit.Transformers;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ExtendDialog : ContentDialog
    {
        //@Content
        public System.Drawing.Size Size => this.SizePicker.Size;
        public IndicatorMode Indicator => this.IndicatorControl.Mode;

        //@Construct
        public ExtendDialog()
        {
            this.InitializeComponent();
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