using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml.Controls;

namespace Luo_Painter.Controls
{
    public sealed partial class CurvePanel : UserControl
    {

        // int i = Index * 2; <para/>
        // A = i % 3; <para/>
        // B = i / 3 % 3; <para/>
        // C = i / 3 / 3 % 3; <para/>
        // 
        // Index: A B C => R G B <para/>
        // 1:  2  0  0 <para/>
        // 2:  1  1  0 <para/>
        // 3:  0  2  0 <para/>
        // 4:  2  2  0 <para/>
        // 5:  1  0  1 <para/>
        // 6:  0  1  1 <para/>
        // 7:  2  1  1 <para/>
        // 8:  1  2  1 <para/>
        // 9:  0  0  2 <para/>
        // 10:  2  0  2 <para/>
        // 11:  1  1  2 <para/>
        // 12:  0  2  2 <para/>
        // 13:  2  2  2 <para/>
        // 14:  1  0  0 <para/>
        // 15:  0  1  0 <para/>
        // 16:  2  1  0 <para/>
        // 17:  1  2  0 <para/>
        // 18:  0  0  1 <para/>
        // 19:  2  0  1 <para/>
        // 20:  1  1  1 <para/>
        // 21:  0  2  1 <para/>
        // 22:  2  2  1 <para/>
        // 23:  1  0  2 <para/>
        // 24:  0  1  2 <para/>
        // 25:  2  1  2 <para/>
        // 26:  1  2  2 <para/>
        // 27:  0  0  0 <para/>

        readonly float[] A = new float[] { 0, 1 };
        readonly float[] B = new float[] { 1, 0 };
        readonly float[] C = new float[] { 0, 0.5f, 1 };

        int Index;

        private void ConstructRandom()
        {
            this.RandomButton.Click += (s, e) =>
            {
                this.Index++;
                this.RedSelector.Reset(this.Red(this.Index));
                this.GreenSelector.Reset(this.Green(this.Index));
                this.BlueSelector.Reset(this.Blue(this.Index));

                switch (this.Mode)
                {
                    case EffectChannelSelect.Red:
                        this.ChangePolyline(this.RedPolyline, this.RedSelector.Data);
                        break;
                    case EffectChannelSelect.Green:
                        this.ChangePolyline(this.GreenPolyline, this.GreenSelector.Data);
                        break;
                    case EffectChannelSelect.Blue:
                        this.ChangePolyline(this.BluePolyline, this.BlueSelector.Data);
                        break;
                    default:
                        break;
                }
                this.Invalidate?.Invoke(this, null); // Delegate
            };
        }

        private float[] Red(int index)
        {
            switch (index % 3)
            {
                case 0: return this.C;
                case 1: return this.B;
                default: return this.A;
            }
        }
        private float[] Green(int index)
        {
            switch (index % 9)
            {
                case 0: return this.A;
                case 1: return this.B;
                case 2: return this.C;
                case 3: return this.C;
                case 4: return this.A;
                case 5: return this.B;
                case 6: return this.B;
                case 7: return this.C;
                case 8: return this.A;
                default: return this.A;
            }
        }
        private float[] Blue(int index)
        {
            switch (index % 12 % 4)
            {
                case 0: return this.A;
                case 1: return this.B;
                default: return this.C;
            }
        }

    }
}