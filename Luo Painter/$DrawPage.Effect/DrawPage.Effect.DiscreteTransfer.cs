using Luo_Painter.UI;

namespace Luo_Painter
{
    public sealed partial class DrawPage
    {

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

        private void SetDiscreteTransfer(NumberPickerMode mode, float e)
        {
        }

    }
}