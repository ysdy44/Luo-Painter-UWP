using Luo_Painter.Layers.Models;
using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Controls
{
    public sealed partial class PaintScrollViewer
    {
        //@Converter
        private double CapOffsetConverter(double value) => 0.8 - 0.04 * value; // 0.4 * (20 - value) / 10;
        private double SegmentOffsetConverter(double value) => 0.2 + 0.04 * value; // 1 - 0.4 * (20 - value) / 10;

        /// <summary> <see cref="BitmapLayer.GetMix(Vector4, float)"/> </summary> 
        private Vector4 GetMix(float mix)
        {
            float x = 1 - mix;
            float y = mix;
            return this.BrushHdr * x + this.PersistenceHdr * y;
        }
        /// <summary> <see cref="BitmapLayer.GetMix(Vector4, float, float)"/> </summary> 
        private Vector4 GetMix(float mix, float persistence)
        {
            float x = 1 - mix;
            float y = mix * (1 - persistence);
            float z = mix * persistence;
            return this.BrushHdr * x + this.MixHdr * y + this.PersistenceHdr * z;
        }

        //@Readonly
        readonly Vector4 BrushHdr = new Vector4(0, 1, 1, 1);
        readonly Vector4 PersistenceHdr = new Vector4(1, 1, 0, 1);
        readonly Vector4 MixHdr = new Vector4(1, 0, 1, 1);

        readonly Color BrushColor = Colors.Aqua;
        readonly Color MixColor = Colors.Fuchsia;
        readonly Color PersistenceColor = Colors.Yellow;

        public void ConstructMix()
        {
            this.MixSlider.ValueChanged += (s, e) =>
            {
                float mix = (float)System.Math.Clamp(e.NewValue / 100, 0, 1);
                float persistence = (float)System.Math.Clamp(this.PersistenceSlider.Value / 100, 0, 1);

                Vector4 cap = this.GetMix(mix);
                this.CapStop.Color = Windows.UI.Color.FromArgb((byte)(cap.W * 255f), (byte)(cap.X * 255f), (byte)(cap.Y * 255f), (byte)(cap.Z * 255f));

                Vector4 segment = this.GetMix(mix, persistence);
                this.SegmentStop.Color = Windows.UI.Color.FromArgb((byte)(segment.W * 255f), (byte)(segment.X * 255f), (byte)(segment.Y * 255f), (byte)(segment.Z * 255f));

                if (this.IsInkEnabled is false) return;
                this.InkPresenter.Mix = mix;
                this.InkType = this.InkPresenter.GetType();
                this.TryInkAsync();
            };
            this.WetSlider.ValueChanged += (s, e) =>
            {
                if (this.IsInkEnabled is false) return;
                double wet = e.NewValue;
                this.InkPresenter.Wet = (float)wet;
                this.TryInkAsync();
            };
            this.PersistenceSlider.ValueChanged += (s, e) =>
            {
                float mix = (float)System.Math.Clamp(this.MixSlider.Value / 100, 0, 1);
                float persistence = (float)System.Math.Clamp(e.NewValue / 100, 0, 1);

                Vector4 segment = this.GetMix(mix, persistence);
                this.SegmentStop.Color = Windows.UI.Color.FromArgb((byte)(segment.W * 255f), (byte)(segment.X * 255f), (byte)(segment.Y * 255f), (byte)(segment.Z * 255f));

                if (this.IsInkEnabled is false) return;
                this.InkPresenter.Persistence = persistence;
                this.TryInkAsync();
            };
        }
    }
}