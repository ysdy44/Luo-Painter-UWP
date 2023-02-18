using FanKit.Transformers;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class ExtendDialog : ContentDialog
    {
        //@Content
        public System.Drawing.Size Size => this.SizePicker.Size;
        public IndicatorMode Indicator
        {
            get
            {
                switch (this.IndicatorPanel.Index)
                {
                    case -4: return IndicatorMode.LeftTop;
                    case -3: return IndicatorMode.Top;
                    case -2: return IndicatorMode.RightTop;

                    case -1: return IndicatorMode.Left;
                    case 0: return IndicatorMode.Center;
                    case 1: return IndicatorMode.Right;

                    case 2: return IndicatorMode.LeftBottom;
                    case 3: return IndicatorMode.Bottom;
                    case 4: return IndicatorMode.RightBottom;

                    default: return IndicatorMode.None;
                }
            }
        }

        //@Construct
        public ExtendDialog()
        {
            this.InitializeComponent();
        }

        public void Resezing(int width, int height)
        {
            this.SizePicker.ResezingWidth(width);
            this.SizePicker.ResezingHeight(height);
        }
    }
}