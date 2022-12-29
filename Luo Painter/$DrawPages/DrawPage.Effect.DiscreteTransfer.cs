using System.Collections.Generic;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

        readonly IList<float> Curves = new List<float>
        {
            0,
            0.5f,
            1
        };

        readonly float[] AlphaTable = new float[] { 0, 1 };
        float[] RedTable => this.CurvePanel.RedTable;
        float[] GreenTable => this.CurvePanel.GreenTable;
        float[] BlueTable => this.CurvePanel.BlueTable;

        public void ConstructDiscreteTransfer()
        {
            this.CurvePanel.Invalidate += (s, e) =>
            {
                this.CanvasVirtualControl.Invalidate(); // Invalidate
            };
        }

        private void SetDiscreteTransfer(NumberPickerMode mode, int e)
        {
        }

    }
}