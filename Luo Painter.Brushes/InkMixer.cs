using System.Numerics;
using Windows.UI;

namespace Luo_Painter.Brushes
{
    public sealed class InkMixer
    {
        Vector4 SourceHdr;
        Color Source;
        public Vector4 ColorHdr => this.SourceHdr;
        public Color Color => this.Source;
        public bool Cache(Color target)
        {
            if (target.A is byte.MinValue)
            {
                this.SourceHdr = Vector4.Zero;
                this.Source = Colors.Transparent;
                return false;
            }
            else if (target.A is byte.MaxValue)
            {
                this.SourceHdr.X = target.R / 255f;
                this.SourceHdr.Y = target.G / 255f;
                this.SourceHdr.Z = target.B / 255f;
                this.Source = target;
            }

            this.SourceHdr.W = target.A / 255f;
            return true;
        }
        public bool Mix(Color target, float opacity)
        {
            if (this.SourceHdr.W is 0f) return this.Cache(target);

            // Opacity
            // 0.0 ~ 1.0

            // 0.0 ~ 0.5
            // 0.5 ~ 1.0

            // Flow
            // 0.48 ~ 0.98

            float flow = opacity / 2f + 0.5f - 0.02f;
            float targetFlow = 1f - flow;

            if (target.A is byte.MinValue)
            {
                this.SourceHdr.W *= flow;
                return false;
            }
            else if (target.A is byte.MaxValue)
            {
                this.SourceHdr.X = this.SourceHdr.X * flow + target.R / 255f * targetFlow;
                this.SourceHdr.Y = this.SourceHdr.Y * flow + target.G / 255f * targetFlow;
                this.SourceHdr.Z = this.SourceHdr.Z * flow + target.B / 255f * targetFlow;
                this.Source.R = (byte)(this.SourceHdr.X * 255);
                this.Source.G = (byte)(this.SourceHdr.Y * 255);
                this.Source.B = (byte)(this.SourceHdr.Z * 255);
            }

            this.SourceHdr.W = this.SourceHdr.W * flow + target.A / 255f * targetFlow;
            this.Source.A = (byte)(this.SourceHdr.W * 255);
            return true;
        }
    }
}