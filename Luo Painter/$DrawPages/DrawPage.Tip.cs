using Luo_Painter.Options;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Luo_Painter
{
    public sealed partial class DrawPage : Page
    {

        private void ConstructTip()
        {
        }

        public void Tip(string title, string subtitle)
        {
            this.ToastTip.Tip(title, subtitle);
        }


        private void ConstructColor()
        {
        }

        private void ConstructColorShape()
        {
            this.ColorButton.ColorChanged += (s, e) =>
            {
                switch (this.OptionType)
                {
                    case OptionType.GradientMapping:
                        this.GradientMappingColorChanged(e.NewColor);
                        break;
                    default:
                        break;
                }
            };
        }

    }
}